using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using mat_process_api.V1.Boundary;
using mat_process_api.V1.Controllers;
using mat_process_api.V1.Domain;
using mat_process_api.V1.Gateways;
using mat_process_api.V1.UseCase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace mat_process_api.Tests.V1.Controllers
{
    [TestFixture]
    public class ProcessDataControllerTests
    {

        private ProcessDataController _processDataController;
        private Mock<IProcessData> _mockUsecase;
        private Faker faker = new Faker();
        private Mock<ILogger<ProcessDataController>> _mockLogger;

        [SetUp]
        public void set_up()
        {
            _mockUsecase = new Mock<IProcessData>();
            _mockLogger = new Mock<ILogger<ProcessDataController>>();
            _processDataController = new ProcessDataController(_mockUsecase.Object, _mockLogger.Object);
        }

        [TestCase("01231-12")]
        [TestCase("324r23/12")]
        [TestCase("34urjkhu7")]
        public void given_a_processRef_when_getprocessdata_method_is_called_the_controller_returns_correct_json_response(string processRef)
        {
            //arange
            var request = new GetProcessDataRequest() { processRef = processRef };
            var response = new GetProcessDataResponse(request, new MatProcessData(), DateTime.Now);
            _mockUsecase.Setup(x=> x.ExecuteGet(It.Is<GetProcessDataRequest>(i => i.processRef == processRef))).Returns(response);
            //act
            var actualResponse = _processDataController.GetProcessData(processRef);
            var okResult = (OkObjectResult)actualResponse;
            var resultContent = (GetProcessDataResponse)okResult.Value;
            //assert
            Assert.NotNull(actualResponse);
            Assert.NotNull(okResult);
            Assert.IsInstanceOf<GetProcessDataResponse>(resultContent);
            Assert.NotNull(resultContent);
            Assert.AreEqual(JsonConvert.SerializeObject(response), JsonConvert.SerializeObject(resultContent));
            Assert.AreEqual(200, okResult.StatusCode);
        }
        [Test]
        public void given_a_processRef_when_getprocessdata_method_is_called_it_then_calls_the_use_case_with_request_object_containing_that_processRef()
        {
            //arrange
            var processRef = faker.Random.Guid().ToString();
            var request = new GetProcessDataRequest { processRef = processRef };
            var response = new GetProcessDataResponse(request, new MatProcessData(), DateTime.Now);
            _mockUsecase.Setup(x => x.ExecuteGet(It.Is<GetProcessDataRequest>(i => i.processRef == processRef))).Returns(response);
            //act
            _processDataController.GetProcessData(processRef);
            //assert
            _mockUsecase.Verify(x => x.ExecuteGet((It.Is<GetProcessDataRequest>(i => i.processRef == processRef))));
        }

  
        [Test]
        public void when_postinitialprocessdocument_method_is_called_then_it_returns_a_response_that_resource_was_created() //temporary test until actual implementation will be worked on.
        {
            //arrange
            int expectedStatusCode = 200;
            //act
            IActionResult controllerResponse = _processDataController.PostInitialProcessDocument();
            OkResult okResult = (OkResult)controllerResponse;
            int actualStatusCode = okResult.StatusCode;
            //assert
            Assert.NotNull(controllerResponse);
            Assert.NotNull(okResult);
            Assert.AreEqual(expectedStatusCode, actualStatusCode);
        }

        #region Update Process data

        [Test]
        public void when_updateexistingprocessdocument_method_is_called_then_it_returns_a_success_response()
        {
            //arrange
            int expectedStatusCode = 200;
            //act
            var controllerResponse = _processDataController.UpdateExistingProcessDocument(new UpdateProcessDataRequest { processDataToUpdate = new MatProcessData() });
            var okResult = (OkObjectResult)controllerResponse;
            var actualStatusCode = okResult.StatusCode;
            //assert
            Assert.NotNull(controllerResponse);
            Assert.NotNull(okResult);
            Assert.AreEqual(expectedStatusCode, actualStatusCode);
        }

        [Test]
        public void ensure_update_controller_calls_use_case_with_request_object()
        {
            //arrange
            var matProcessData = new MatProcessData();
            //add fields to be updated
            matProcessData.ProcessData = new
            {
                firstField = faker.Random.Word()
            };
            //set up Guid to filder by
            matProcessData.Id = faker.Random.Guid().ToString();
            var request = new UpdateProcessDataRequest() { processDataToUpdate = matProcessData };
            var actualResponse = _processDataController.UpdateExistingProcessDocument(request);
            //verify usecase is called with an update process request object
            _mockUsecase.Verify(x => x.ExecuteUpdate((It.Is<UpdateProcessDataRequest>(i => i == request))));
        }

        [Test]
        public void given_an_update_object_when_update_process_data_method_is_called_the_controller_returns_correct_json_response()
        {
            //arange
            var matProcessData = new MatProcessData();
            //add fields to be updated
            matProcessData.ProcessData = new
            {
                firstField = faker.Random.Word(),
                anyField = faker.Random.Words(),
                numberField = faker.Random.Number()
            };
            //set up Guid to filder by
            matProcessData.Id = faker.Random.Guid().ToString();

            var request = new UpdateProcessDataRequest() { processDataToUpdate = matProcessData };
            var response = new UpdateProcessDataResponse(request, new MatProcessData(), DateTime.Now);
            _mockUsecase.Setup(x => x.ExecuteUpdate(request)).Returns(response);
            //act
            var actualResponse = _processDataController.UpdateExistingProcessDocument(request);
            var okResult = (OkObjectResult)actualResponse;
            var resultContent = (UpdateProcessDataResponse)okResult.Value;
            //assert
            Assert.NotNull(actualResponse);
            Assert.NotNull(okResult);
            Assert.IsInstanceOf<UpdateProcessDataResponse>(resultContent);
            Assert.NotNull(resultContent);
            Assert.AreEqual(JsonConvert.SerializeObject(response), JsonConvert.SerializeObject(resultContent));
            Assert.AreEqual(200, okResult.StatusCode);
        }
        //TODO FINISH OFF WHEN VALIDATOR IS ADDED
        [Test]
        public void given_an_invalid_request_object_controller_should_return_bad_request()
        {
            var requestObject = new MatProcessData();
            var request = new UpdateProcessDataRequest() { processDataToUpdate = new MatProcessData() };

            var response = _processDataController.UpdateExistingProcessDocument(request);
            var okResult = (OkObjectResult)response;
            var resultContent = (UpdateProcessDataResponse)okResult.Value;

            Assert.NotNull(response);
            Assert.NotNull(okResult);
            Assert.NotNull(resultContent);
            Assert.AreEqual(JsonConvert.SerializeObject(response), JsonConvert.SerializeObject(resultContent));
            Assert.AreEqual(400, okResult.StatusCode);
        }

        [Test]
        public void test_that_controller_returns_correct_message_and_status_code_when_no_document_was_found_for_update()
        {
            //arange
            var matProcessData = new MatProcessData();
            //add fields to be updated
            matProcessData.ProcessData = new
            {
                firstField = faker.Random.Word(),
                anyField = faker.Random.Words(),
                numberField = faker.Random.Number()
            };
            //set up Guid to filder by
            matProcessData.Id = faker.Random.Guid().ToString();
            var errorResponse = $"Document with reference {matProcessData.Id} was not found in the database." +
                    $" An update is not possible on non-existent documents.";
            var request = new UpdateProcessDataRequest() { processDataToUpdate = matProcessData };
            _mockUsecase.Setup(x => x.ExecuteUpdate(request)).Throws<DocumentNotFound>();
            //act
            var actualResponse = _processDataController.UpdateExistingProcessDocument(request);
            var result = (ObjectResult)actualResponse;
            var resultContent = result.Value;
            //assert
            Assert.NotNull(actualResponse);
            Assert.NotNull(result);
            Assert.NotNull(resultContent);
            Assert.AreEqual(errorResponse, resultContent);
            Assert.AreEqual(200, result.StatusCode);
        }
        [Test]
        public void test_that_controller_returns_correct_500_status_code_when_an_error_has_occured()
        {
            //arange
            var matProcessData = new MatProcessData();
            //add fields to be updated
            matProcessData.ProcessData = new
            {
                firstField = faker.Random.Word(),
                anyField = faker.Random.Words(),
                numberField = faker.Random.Number()
            };
            //set up Guid to filder by
            matProcessData.Id = faker.Random.Guid().ToString();
            var errorResponse = $"Document with reference {matProcessData.Id} was not found in the database." +
                    $" An update is not possible on non-existent documents.";
            var request = new UpdateProcessDataRequest() { processDataToUpdate = matProcessData };
            _mockUsecase.Setup(x => x.ExecuteUpdate(request)).Throws<AggregateException>();
            //act
            var actualResponse = _processDataController.UpdateExistingProcessDocument(request);
            var result = (ObjectResult)actualResponse;
            var resultContent = result.Value;
            //assert
            Assert.NotNull(actualResponse);
            Assert.NotNull(result);
            Assert.NotNull(resultContent);
            Assert.AreEqual(500, result.StatusCode);
        }
        #endregion
    }
}
