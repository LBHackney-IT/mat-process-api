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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

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
            Mock<ILogger<ProcessDataController>> logger = new Mock<ILogger<ProcessDataController>>();

            _processDataController = new ProcessDataController(processDataUsecase, logger.Object);
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
        public void postInitialProcessDocument_controller_method_end_to_end_test_with_a_valid_request()
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

        //invalid request - needs validation setup first

        //duplicate request - needs error handling in the gateway setup

        #endregion
    }
}
