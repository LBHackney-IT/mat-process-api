using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using mat_process_api.Tests.V1.Helper;
using mat_process_api.V1.Boundary;
using mat_process_api.V1.Controllers;
using mat_process_api.V1.Domain;
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
        #region Setup

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
            var updateDocValidator = new UpdateProcessDocumentRequestValidator();
            var getDocValidator = new GetProcessDocumentRequestValidator();
            Mock<ILogger<ProcessDataController>> logger = new Mock<ILogger<ProcessDataController>>();

            _processDataController = new ProcessDataController(processDataUsecase, logger.Object, postInitDocValidator, updateDocValidator, getDocValidator);
        }

        #endregion
        #region Get Process Document (end to end)

        [Test]
        public void given_a_valid_and_existing_processRef_when_GetProcessData_controller_method_is_called_then_it_returns_a_200_Ok_result()
        {
            //arrange
            int expectedStatusCode = 200;
            var existingDocument = MatProcessDataHelper.CreateProcessDataObject();
            _dbcontext.getCollection().InsertOne(BsonDocument.Parse(JsonConvert.SerializeObject(existingDocument))); // insert expected document, which we expect the gateway to retrieve
            var request = new GetProcessDataRequest() { processRef = existingDocument.Id }; //valid, and existing

            //act
            var contollerResponse = _processDataController.GetProcessData(request);
            var controllerObjectResult = contollerResponse as ObjectResult;

            //assert
            Assert.NotNull(contollerResponse);
            Assert.IsInstanceOf<OkObjectResult>(controllerObjectResult);
            Assert.AreEqual(expectedStatusCode, controllerObjectResult.StatusCode);
        }

        [Test]
        public void given_a_valid_and_existing_processRef_when_GetProcessData_controller_method_is_called_then_it_returns_matching_document_from_the_database()
        {
            //arrange
            var expectedDocument = MatProcessDataHelper.CreateProcessDataObject();
            string expectedProcessRef = expectedDocument.Id;
            _dbcontext.getCollection().InsertOne(BsonDocument.Parse(JsonConvert.SerializeObject(expectedDocument))); // insert expected document, which we expect the gateway to retrieve

            GetProcessDataRequest request = new GetProcessDataRequest() { processRef = expectedProcessRef };

            //act
            var response = _processDataController.GetProcessData(request);
            var okResult = (OkObjectResult)response;
            var getProcessDataResponse = okResult.Value as GetProcessDataResponse;
            var retrievedDocument = getProcessDataResponse.ProcessData;

            //assert
            Assert.IsInstanceOf<MatProcessData>(retrievedDocument);
            Assert.NotNull(getProcessDataResponse.ProcessData);
            Assert.NotNull(getProcessDataResponse.ProcessData.ProcessType);

            Assert.That(
                expectedProcessRef == retrievedDocument.Id &&
                expectedDocument.ProcessType.name == retrievedDocument.ProcessType.name &&
                expectedDocument.ProcessType.value == retrievedDocument.ProcessType.value &&
                expectedDocument.DateCreated == retrievedDocument.DateCreated &&
                expectedDocument.DateLastModified == retrievedDocument.DateLastModified &&
                expectedDocument.DateCompleted == retrievedDocument.DateCompleted &&
                expectedDocument.ProcessDataAvailable == retrievedDocument.ProcessDataAvailable &&
                expectedDocument.ProcessDataSchemaVersion == retrievedDocument.ProcessDataSchemaVersion &&
                expectedDocument.ProcessStage == retrievedDocument.ProcessStage &&
                expectedDocument.LinkedProcessId == retrievedDocument.LinkedProcessId
                );
        }

        [Test]
        public void given_an_invalid_processRef_when_GetProcessData_controller_method_is_called_then_it_returns_a_400_BadRequest_result()
        {
            //arrange
            int expectedStatusCode = 400;
            var request = new GetProcessDataRequest() { processRef = _faker.Random.Word() }; //processRef of invalid format

            //act
            var contollerResponse = _processDataController.GetProcessData(request);
            var controllerObjectResult = contollerResponse as ObjectResult;

            //assert
            Assert.NotNull(contollerResponse);
            Assert.IsInstanceOf<BadRequestObjectResult>(controllerObjectResult);
            Assert.AreEqual(expectedStatusCode, controllerObjectResult.StatusCode);
        }

        [Test]
        public void given_an_valid_and_nonexisting_processRef_when_GetProcessData_controller_method_is_called_then_it_returns_a_404_resource_NotFound_result()
        {
            //arrange
            int expectedStatusCode = 404;
            var request = new GetProcessDataRequest() { processRef = _faker.Random.Guid().ToString() }; //valid, but there's no instance of it in DB

            //act
            var contollerResponse = _processDataController.GetProcessData(request);
            var controllerObjectResult = contollerResponse as ObjectResult;

            //assert
            Assert.NotNull(contollerResponse);
            Assert.IsInstanceOf<NotFoundObjectResult>(controllerObjectResult);
            Assert.AreEqual(expectedStatusCode, controllerObjectResult.StatusCode);
        }

        //Not sure how to test 500 result.

        #endregion

        [TestCase("00000000-0000-0000-0000-000000000000")]
        [TestCase("00000000-dd23-0000-abcd-000000000000")]
        [TestCase("2539ca27-12c0-e811-a96c-002248072cb4")]
        public void update_process_controller_end_to_end_test(string processRef)
        {
            //arrange
            var dataToInsert = MatProcessDataHelper.CreateProcessDataObject();
            dataToInsert.Id = processRef;
            _dbcontext.getCollection().InsertOne(BsonDocument.Parse(JsonConvert.SerializeObject(dataToInsert)));
            //fields to update
            var dataToUpdate = new MatUpdateProcessData();
            dataToUpdate.DateLastModified = DateTime.Now;
            dataToUpdate.PreProcessData = new
            {
                randomField = "abc"
            };
            var request = new UpdateProcessDataRequest() { processRef = processRef, processDataToUpdate = dataToUpdate };
            //act
            var response = _processDataController.UpdateExistingProcessDocument(request);
            var okResult = (OkObjectResult)response;
            var updateProcessDataResponse = okResult.Value as UpdateProcessDataResponse;
            //assert
            Assert.IsInstanceOf<MatProcessData>(updateProcessDataResponse.UpdatedProcessData);
            Assert.AreEqual(processRef, updateProcessDataResponse.UpdatedProcessData.Id);
            Assert.AreEqual(dataToUpdate.DateLastModified.ToShortDateString(),
                updateProcessDataResponse.UpdatedProcessData.DateLastModified.ToShortDateString());
            Assert.AreEqual(JsonConvert.SerializeObject(dataToUpdate.PreProcessData),
                JsonConvert.SerializeObject(updateProcessDataResponse.UpdatedProcessData.PreProcessData));
        }
        [Test]
        public void update_process_controller_end_to_end_testing_for_bad_request_given_invalid_request()
        {
            //arrange
            var dataToUpdate = new MatUpdateProcessData();
            var processRef = _faker.Random.Word();
   
            var request = new UpdateProcessDataRequest() { processRef = processRef,processDataToUpdate = dataToUpdate };
            //act
            var response = _processDataController.UpdateExistingProcessDocument(request);
            var okResult = (ObjectResult)response;
            //assert
            Assert.AreEqual(400, okResult.StatusCode); //check if it was a bad request
        }
        [Test]
        public void update_process_controller_end_to_end_testing_error_message_when_document_is_not_found()
        {
            //arrange
            var dataToUpdate = new MatUpdateProcessData();
            var processRef = _faker.Random.Guid().ToString();
            dataToUpdate.PreProcessData = new
            {
                randomField = "abc"
            };

            var request = new UpdateProcessDataRequest() { processDataToUpdate = dataToUpdate, processRef = processRef };
            //act
            var response = _processDataController.UpdateExistingProcessDocument(request);
            var okResult = (ObjectResult)response;
            var error = okResult.Value;
            //assert
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual($"Document with reference { processRef} was not found in the database." +
                        $" An update is not possible on non-existent documents.", error);
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
