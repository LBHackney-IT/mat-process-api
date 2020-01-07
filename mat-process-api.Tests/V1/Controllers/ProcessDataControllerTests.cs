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
using mat_process_api.V1.Gateways;
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
        public void given_postInitialProcessDocument_controller_method_call_when_request_validation_fails_then_the_controller_returns_correct_badRequest_response()
        {
            //arrange
            int errorCount = faker.Random.Int(1,10); //simulate from 1 to 10 validation errors (triangulation).
            var validationErrorList = new List<ValidationFailure>(); //this list will be used as constructor argument for 'ValidationResult'.
            for (int i = errorCount; i > 0; i--) { validationErrorList.Add(new ValidationFailure(faker.Random.Word(), faker.Random.Word())); } //generate from 1 to 10 fake validation errors. Single line for-loop so that it wouldn't distract from what's key in this test.

            var fakeValidationResult = new FV.ValidationResult(validationErrorList); //Need to create ValidationResult so that I could setup Validator mock to return it upon '.Validate()' call. Also this is the only place where it's possible to manipulate the validation result - You can only make the validation result invalid by inserting a list of validation errors as a parameter through a constructor. The boolean '.IsValid' comes from expression 'IsValid => Errors.Count == 0;', so it can't be set manually.
            _mockValidator.Setup(v => v.Validate(It.IsAny<PostInitialProcessDocumentRequest>())).Returns(fakeValidationResult);

            //act
            var controllerResponse = _processDataController.PostInitialProcessDocument(new PostInitialProcessDocumentRequest());
            var result = (ObjectResult)controllerResponse;
            var resultContents = (IList<ValidationFailure>)result.Value;

            //assert
            Assert.NotNull(controllerResponse);
            Assert.IsInstanceOf<BadRequestObjectResult>(result); // should be bad request, since validation fails.
            Assert.NotNull(result);
            Assert.IsInstanceOf<IList<ValidationFailure>>(resultContents);
            Assert.NotNull(resultContents);

            Assert.AreEqual(errorCount, resultContents.Count); // there should be 1 validation failure coming from the invalid guid
            Assert.AreEqual(400, result.StatusCode); // expecting bad request result
        }

        [Test]
        public void given_request_with_unique_key_that_already_exists_when_postInitialProcessDocument_controller_method_is_called_then_upon_catching_custom_conflict_exception_the_correct_conflict_result_is_returned() //testing exception handling. in this case testing how our custom exception bubbles up from gateway. chose conflict exception to test 2 things at once - exception handling + custom exception wrapping.
        {
            //arrange
            //setup mocks:
            _mockValidator.Setup(v => v.Validate(It.IsAny<PostInitialProcessDocumentRequest>())).Returns(new FV.ValidationResult()); //set up mock validation to return Validation with no errors.
            _mockUsecase.Setup(g => g.ExecutePost(It.IsAny<PostInitialProcessDocumentRequest>())).Returns(() => throw new ConflictException(faker.Random.Word(), new Exception(faker.Random.Word())));

            //setup double insert:
            string conflictGuid = faker.Random.Guid().ToString(); //This is the conflicting Guid, that will be repeated accross 2 request objects.

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

        #endregion
    }
}
