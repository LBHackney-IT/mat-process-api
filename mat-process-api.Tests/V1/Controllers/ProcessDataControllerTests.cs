using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FluentValidation.Results;
using FV = FluentValidation.Results; //give alias to namespace - shortens the 'ValidationResult' reference.
using mat_process_api.Tests.V1.Helper;
using mat_process_api.V1.Boundary;
using mat_process_api.V1.Controllers;
using mat_process_api.V1.Domain;
using mat_process_api.V1.UseCase;
using mat_process_api.V1.Validators;
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
        private Mock<IPostInitialProcessDocumentRequestValidator> _mockValidator;

        [SetUp]
        public void set_up()
        {
            _mockUsecase = new Mock<IProcessData>();
            _mockLogger = new Mock<ILogger<ProcessDataController>>();
            _mockValidator = new Mock<IPostInitialProcessDocumentRequestValidator>();
            _processDataController = new ProcessDataController(_mockUsecase.Object, _mockLogger.Object, _mockValidator.Object);
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

        #region Post Initial Process Document

        [Test]
        public void given_a_valid_request_when_postinitialprocessdocument_controller_method_is_called_then_it_returns_a_response_that_resource_was_created() //temporary test until actual implementation will be worked on.
        {
            //arrange
            int expectedStatusCode = 201;
            PostInitialProcessDocumentRequest requestObject = MatProcessDataHelper.CreatePostInitialProcessDocumentRequestObject();
            _mockValidator.Setup(v => v.Validate(It.IsAny<PostInitialProcessDocumentRequest>())).Returns(new FV.ValidationResult()); //set up mock validation to return Validation with no errors.

            //act
            IActionResult controllerResponse = _processDataController.PostInitialProcessDocument(requestObject);
            var result = (ObjectResult)controllerResponse;
            var actualStatusCode = result.StatusCode;

            //assert
            Assert.NotNull(controllerResponse);
            Assert.NotNull(result);
            Assert.AreEqual(expectedStatusCode, actualStatusCode);
        }

        [Test]
        public void given_a_postInitialProcessDocumentRequest_when_postInitialProcessDocument_controller_method_is_called_it_then_calls_the_use_case_while_passing_the_request_object_to_it()
        {
            //arrange
            PostInitialProcessDocumentRequest requestObject = MatProcessDataHelper.CreatePostInitialProcessDocumentRequestObject();
            _mockValidator.Setup(v => v.Validate(It.IsAny<PostInitialProcessDocumentRequest>())).Returns(new FV.ValidationResult()); //set up mock validation to return Validation with no errors.
            _mockUsecase.Setup(g => g.ExecutePost(It.IsAny<PostInitialProcessDocumentRequest>()));

            //act
            _processDataController.PostInitialProcessDocument(requestObject);

            //assert
            _mockUsecase.Verify(x => x.ExecutePost(
                It.Is<PostInitialProcessDocumentRequest>(req => req.processRef == requestObject.processRef)
                ), Times.Once);
        }

        [Test]
        public void given_a_request_when_postInitialProcessDocument_controller_method_is_called_then_it_calls_the_validator_with_that_request()
        {
            //arrange
            PostInitialProcessDocumentRequest request = MatProcessDataHelper.CreatePostInitialProcessDocumentRequestObject();
            _mockValidator.Setup(v => v.Validate(It.IsAny<PostInitialProcessDocumentRequest>())).Returns(new FV.ValidationResult()); //set up mock validation to return Validation with no errors.
            _mockUsecase.Setup(g => g.ExecutePost(It.IsAny<PostInitialProcessDocumentRequest>()));

            //act
            _processDataController.PostInitialProcessDocument(request);

            //assert
            _mockValidator.Verify(v => v.Validate(It.Is<PostInitialProcessDocumentRequest>(i =>
                i.processRef == request.processRef &&
                i.processType.name == request.processType.name &&
                i.processType.value == request.processType.value &&
                i.processDataSchemaVersion == request.processDataSchemaVersion
                )), Times.Once);
        }

        [Test]
        public void given_a_valid_postInitialProcessDocumentRequest_when_postInitialProcessDocument_controller_method_is_called_then_the_controller_returns_correct_json_response()
        {
            //arange
            PostInitialProcessDocumentRequest requestObject = MatProcessDataHelper.CreatePostInitialProcessDocumentRequestObject();
            _mockValidator.Setup(v => v.Validate(It.IsAny<PostInitialProcessDocumentRequest>())).Returns(new FV.ValidationResult()); //set up mock validation to return Validation with no errors.

            var expectedResponse = new PostInitialProcessDocumentResponse(requestObject, requestObject.processRef, DateTime.Now);
            _mockUsecase.Setup(g => g.ExecutePost(It.IsAny<PostInitialProcessDocumentRequest>())).Returns(expectedResponse);

            ////act
            var actualResponse = _processDataController.PostInitialProcessDocument(requestObject);
            var result = (ObjectResult)actualResponse;
            var resultContent = (PostInitialProcessDocumentResponse)result.Value;

            ////assert
            Assert.NotNull(actualResponse);
            Assert.NotNull(result);
            Assert.IsInstanceOf<PostInitialProcessDocumentResponse>(resultContent);
            Assert.NotNull(resultContent);
            Assert.AreEqual(expectedResponse.ProcessRef, resultContent.ProcessRef);
            Assert.AreEqual(expectedResponse.GeneratedAt, resultContent.GeneratedAt);
            Assert.AreEqual(expectedResponse.Request.processRef, resultContent.Request.processRef);
            Assert.AreEqual(JsonConvert.SerializeObject(expectedResponse), JsonConvert.SerializeObject(resultContent));
        }

        //There should be test for given invalid request... essentially invalid GUID, but it should be done after the validation is done.

        #endregion
    }
}
