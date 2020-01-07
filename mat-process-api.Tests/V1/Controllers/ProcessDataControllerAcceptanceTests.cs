using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mat_process_api.Tests.V1.Helper;
using mat_process_api.V1.Boundary;
using mat_process_api.V1.Controllers;
using mat_process_api.V1.Domain;
using mat_process_api.V1.Gateways;
using mat_process_api.V1.Infrastructure;
using mat_process_api.V1.UseCase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace mat_process_api.Tests.V1.Controllers
{
    public class ProcessDataControllerAcceptanceTests
    {
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

        [TestCase("sefwefff")]
        [TestCase("34252345tff")]
        [TestCase("324t5gehff")]
        public void controller_end_to_end_test(string processRef)
        {
            //arrange
            var dataToInsert = MatProcessDataHelper.CreateProcessDataObject();
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
        [TestCase("00000000-0000-0000-0000-000000000000")]
        [TestCase("00000000-dd23-0000-abcd-000000000000")]
        [TestCase("00000000-pnhm-0000-1234-00000bb00000")]
        public void update_process_controller_end_to_end_test(string processRef)
        {
            //arrange
            var dataToInsert = MatProcessDataHelper.CreateProcessDataObject();
            dataToInsert.Id = processRef;
            _dbcontext.getCollection().InsertOne(BsonDocument.Parse(JsonConvert.SerializeObject(dataToInsert)));
            //fields to update
            dataToInsert.DateLastModified = DateTime.Now;
            dataToInsert.PreProcessData = new
            {
                randomField = "abc"
            };
            var request = new UpdateProcessDataRequest() { processDataToUpdate = dataToInsert };
            //act
            var response = _processDataController.UpdateExistingProcessDocument(request);
            var okResult = (OkObjectResult)response;
            var updateProcessDataResponse = okResult.Value as UpdateProcessDataResponse;
            //assert
            Assert.IsInstanceOf<MatProcessData>(updateProcessDataResponse.ProcessData);
            Assert.AreEqual(processRef, updateProcessDataResponse.ProcessData.Id);
            Assert.AreEqual(dataToInsert.DateLastModified.ToShortDateString(),
                updateProcessDataResponse.ProcessData.DateLastModified.ToShortDateString());
            Assert.AreEqual(JsonConvert.SerializeObject(dataToInsert.PreProcessData),
                JsonConvert.SerializeObject(updateProcessDataResponse.ProcessData.PreProcessData));
        }

        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
            //clear collection - remove any documents inserted during thet test
            _dbcontext.getCollection().DeleteMany(Builders<BsonDocument>.Filter.Empty);
        }
    }
}
