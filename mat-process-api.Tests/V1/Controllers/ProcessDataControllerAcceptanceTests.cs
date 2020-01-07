using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using mat_process_api.Tests.V1.Helper;
using mat_process_api.V1.Boundary;
using mat_process_api.V1.Controllers;
using mat_process_api.V1.Domain;
using mat_process_api.V1.Factories;
using mat_process_api.V1.Gateways;
using mat_process_api.V1.Infrastructure;
using mat_process_api.V1.UseCase;
using mat_process_api.V1.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using FluentValidation.Results;

namespace mat_process_api.Tests.V1.Controllers
{
    public class ProcessDataControllerAcceptanceTests
    {
        private readonly Faker _faker = new Faker();
        private ProcessDataController _processDataController;
        private MatDbContext _dbcontext;

        [SetUp]
        public void set_up()
        {
            //setting connection options for dbcontext
            var mockOptions = new Mock<IOptions<ConnectionSettings>>();
            //runtime DB connection strings
            var settings = new ConnectionSettings
            {
                ConnectionString = Environment.GetEnvironmentVariable("MONGO_CONN_STRING") ??
                                   @"mongodb://localhost:1433/",
                CollectionName = "process-data",
                Database = "mat-processes"
            };
            //make options mock return the object with settings
            mockOptions.SetupGet(x => x.Value).Returns(settings);

            //setting up dbcontext
            _dbcontext = new MatDbContext(mockOptions.Object);
            var processDataGateway = new ProcessDataGateway(_dbcontext);
            var processDataUsecase = new ProcessDataUseCase(processDataGateway);
            var postInitDocValidator = new PostInitialProcessDocumentRequestValidator();
            Mock<ILogger<ProcessDataController>> logger = new Mock<ILogger<ProcessDataController>>();

            _processDataController = new ProcessDataController(processDataUsecase, logger.Object, postInitDocValidator);
        }

        [Test]
        public void controller_end_to_end_test()
        {
            //arrange
            var dataToInsert = MatProcessDataHelper.CreateProcessDataObject();
            string processRef = _faker.Random.Guid().ToString();
            dataToInsert.Id = processRef;
            _dbcontext.getCollection().InsertOne(BsonDocument.Parse(JsonConvert.SerializeObject(dataToInsert)));
            //act
            var response = _processDataController.GetProcessData(processRef);
            var okResult = (OkObjectResult)response;
            //assert
            var getProcessDataResponse = okResult.Value as GetProcessDataResponse;
            Assert.IsInstanceOf<MatProcessData>(getProcessDataResponse.ProcessData);
            Assert.AreEqual(processRef,getProcessDataResponse.ProcessData.Id);
        }

        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
            //clear collection - remove any documents inserted during thet test
            _dbcontext.getCollection().DeleteMany(Builders<BsonDocument>.Filter.Empty);
        }

        #region controller Post Initial Process Document endpoint

        [Test]
        public void postInitialProcessDocument_controller_method_end_to_end_test_with_a_valid_request() //testing regular response
        {
            //arrange
            PostInitialProcessDocumentRequest requestObject = MatProcessDataHelper.CreatePostInitialProcessDocumentRequestObject();
            var expectedResponse = new PostInitialProcessDocumentResponse(requestObject, requestObject.processRef, DateTime.Now);

            //act
            var controllerResponse = _processDataController.PostInitialProcessDocument(requestObject);
            var result = (ObjectResult)controllerResponse;
            var resultContent = (PostInitialProcessDocumentResponse)result.Value;

            //assert
            //check if the doc got inserted
            var documentFromDB = BsonSerializer.Deserialize<MatProcessData>(_dbcontext.getCollection().FindAsync(Builders<BsonDocument>.Filter.Eq("_id", requestObject.processRef)).Result.FirstOrDefault());
            Assert.NotNull(documentFromDB);
            Assert.AreEqual(requestObject.processRef, documentFromDB.Id);
            Assert.NotNull(documentFromDB.ProcessType);
            Assert.AreEqual(requestObject.processType.value, documentFromDB.ProcessType.value);
            Assert.AreEqual(requestObject.processType.name, documentFromDB.ProcessType.name);
            Assert.AreEqual(requestObject.processDataSchemaVersion, documentFromDB.ProcessDataSchemaVersion);

            //check the controller response
            Assert.NotNull(controllerResponse);
            Assert.IsInstanceOf<ObjectResult>(result);
            Assert.NotNull(result);
            Assert.IsInstanceOf<PostInitialProcessDocumentResponse>(result.Value);
            Assert.NotNull(result.Value);
            Assert.AreEqual(expectedResponse.ProcessRef, resultContent.ProcessRef);
            Assert.AreEqual(expectedResponse.Request.processRef, resultContent.Request.processRef);

            //check status code
            Assert.AreEqual(201, result.StatusCode);
        }

        [Test]
        public void postInitialProcessDocument_controller_method_end_to_end_test_with_a_invalid_request() //testing, when validation fails
        {
            //arrange
            PostInitialProcessDocumentRequest requestObject = MatProcessDataHelper.CreatePostInitialProcessDocumentRequestObject();
            string nonsenseGuid = _faker.Random.Word().ToString(); // invalid Guid (not even a guid) - this should trip the validation.
            requestObject.processRef = nonsenseGuid;

            var postValidator = new PostInitialProcessDocumentRequestValidator();
            var isRequestValid = postValidator.Validate(requestObject);
            var errors = isRequestValid.Errors;

            var expectedResponse = new BadRequestObjectResult(errors);

            //act
            var controllerResponse = _processDataController.PostInitialProcessDocument(requestObject);
            var actualResult = (ObjectResult)controllerResponse;
            var actualContents = (IList<ValidationFailure>)actualResult.Value;

            //assert
            Assert.NotNull(controllerResponse);
            Assert.IsInstanceOf<BadRequestObjectResult>(actualResult); // should be bad request, since validation fails.
            Assert.NotNull(actualResult);
            Assert.IsInstanceOf<IList<ValidationFailure>>(actualContents);
            Assert.NotNull(actualContents);

            Assert.AreEqual(1, actualContents.Count); // there should be 1 validation failure coming from the invalid guid
            Assert.AreEqual(400, actualResult.StatusCode); // expecting bad request result

            //the following simply proves that validation errors get wraped up properly.
            Assert.AreEqual(((IList<ValidationFailure>)expectedResponse.Value)[0].ErrorMessage, actualContents[0].ErrorMessage);
        }

        [Test]
        public void postInitialProcessDocument_controller_method_end_to_end_test_with_a_conflict_exception_in_the_gateway() //testing exception handling. in this case testing how our custom exception bubbles up from gateway. chose conflict exception to test 2 things at once - exception handling + custom exception wrapping.
        {
            //arrange
            string conflictGuid = _faker.Random.Guid().ToString(); //This is the conflicting Guid, that will be repeated accross 2 request objects.

            PostInitialProcessDocumentRequest requestObject1 = MatProcessDataHelper.CreatePostInitialProcessDocumentRequestObject();
            requestObject1.processRef = conflictGuid; // set conflict Guid.
            _processDataController.PostInitialProcessDocument(requestObject1); //The process document with conflict Guid identifier has been inserted into the database.

            PostInitialProcessDocumentRequest requestObject2 = MatProcessDataHelper.CreatePostInitialProcessDocumentRequestObject();
            requestObject2.processRef = conflictGuid; // set conflict Guid.

            //act
            var controllerResponse = _processDataController.PostInitialProcessDocument(requestObject2); //The attempt to insert another document into the database with the already existing identifier (conflict guid). Conflict happens.
            var actualResult = (ObjectResult)controllerResponse;
            var errorMessage = (string)actualResult.Value;

            //assert
            Assert.NotNull(controllerResponse);
            Assert.IsInstanceOf<ConflictObjectResult>(actualResult);
            Assert.NotNull(actualResult);
            Assert.IsInstanceOf<string>(errorMessage);
            Assert.NotNull(errorMessage);
            Assert.IsNotEmpty(errorMessage);

            Assert.AreEqual(409, actualResult.StatusCode);
            Assert.AreNotEqual("An error inserting an object with duplicate key has occured - ", errorMessage); //If they're equal, then it means that empty exception has been appended to this sentense.
            //There is no good way to check the error message, since it would require to cause the exception manually through writing error handling implementation in the test.
        }

        #endregion
    }
}
