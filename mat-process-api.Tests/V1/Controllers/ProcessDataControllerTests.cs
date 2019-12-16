using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using mat_process_api.V1.Boundary;
using mat_process_api.V1.Controllers;
using mat_process_api.V1.Domain;
using mat_process_api.V1.UseCase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.IO;
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

        public void given_a_processRef_when_getprocessdata_method_is_called_the_controller_returns_correct_json_response()
        {
            //arange
            string processRef = faker.Random.Guid().ToString();
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
            string processRef = faker.Random.Guid().ToString();
            var request = new GetProcessDataRequest { processRef = processRef };
            var response = new GetProcessDataResponse(request, new MatProcessData(), DateTime.Now);
            _mockUsecase.Setup(x => x.ExecuteGet(It.Is<GetProcessDataRequest>(i => i.processRef == processRef))).Returns(response);
            //act
            _processDataController.GetProcessData(processRef);
            //assert
            _mockUsecase.Verify(x => x.ExecuteGet((It.Is<GetProcessDataRequest>(i => i.processRef == processRef))));
        }

        [Test]
        public void when_updateexistingprocessdocument_method_is_called_then_it_returns_a_success_response()
        {
            //arrange
            int expectedStatusCode = 200; //status code not yet decided, so I go with the 200 for now
            //act
            IActionResult controllerResponse = _processDataController.UpdateExistingProcessDocument();
            OkResult okResult = (OkResult)controllerResponse;
            int actualStatusCode = okResult.StatusCode;
            //assert
            Assert.NotNull(controllerResponse);
            Assert.NotNull(okResult);
            Assert.AreEqual(expectedStatusCode, actualStatusCode);
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
    }
}
