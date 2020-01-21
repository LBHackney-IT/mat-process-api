using mat_process_api.V1.Boundary;
using mat_process_api.V1.Controllers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using System.Threading.Tasks;
using mat_process_api.V1.UseCase;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using mat_process_api.V1.Validators;
using mat_process_api.Tests.V1.Helper;
using FluentValidation.Results;
using FV = FluentValidation.Results;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging.Internal;
using mat_process_api.V1.Exceptions;

namespace mat_process_api.Tests.V1.Controllers
{
    [TestFixture]
    public class ProcessDataImagesControllerTests
    {
        private ProcessImageController _processImageController;
        private Mock<ILogger<ProcessImageController>> _mockLogger;
        private Mock<IProcessImageUseCase> _mockUsecase;
        private Mock<IPostProcessImageRequestValidator> _mockPostValidator;
        private Mock<IGetProcessImageRequestValidator> _mockGetValidator;
        private Faker _faker = new Faker();

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<ProcessImageController>>();
            _mockUsecase = new Mock<IProcessImageUseCase>();
            _mockPostValidator = new Mock<IPostProcessImageRequestValidator>();
            _mockGetValidator = new Mock<IGetProcessImageRequestValidator>();
            _processImageController = new ProcessImageController(_mockUsecase.Object, _mockPostValidator.Object, _mockGetValidator.Object, _mockLogger.Object);
        }

        #region Post Process Image

        [Test]
        public void given_valid_request_when_postProcessImage_controller_method_is_called_then_usecase_is_called()
        {
            //arrange
            var request = new PostProcessImageRequest();
            _mockPostValidator.Setup(x => x.Validate(request)).Returns(new FV.ValidationResult()); //setup validator to return a no error validation result

            //act
            _processImageController.PostProcessImage(request);

            //assert
            _mockUsecase.Verify(u => u.ExecutePost(It.IsAny<PostProcessImageRequest>()), Times.Once);
        }

        [Test]
        public void given_valid_request_when_postProcessImage_controller_method_is_called_then_usecase_is_called_with_correct_data()
        {
            //arange
            var request = MatProcessDataHelper.CreatePostProcessImageRequestObject();
            _mockPostValidator.Setup(x => x.Validate(request)).Returns(new FV.ValidationResult()); //setup validator to return a no error validation result

            //act
            _processImageController.PostProcessImage(request);

            //assert
            _mockUsecase.Verify(u => u.ExecutePost(It.Is<PostProcessImageRequest>(obj =>
                obj.processRef == request.processRef &&
                obj.imageId == request.imageId &&
                obj.base64Image == request.base64Image
                )), Times.Once);
        }

        [Test]
        public void given_valid_request_when_postProcessImage_controller_method_is_called_then_it_returns_204_NoContent_result()
        {
            //arrange
            var expectedStatusCode = 204;
            var request = new PostProcessImageRequest();
            _mockPostValidator.Setup(x => x.Validate(request)).Returns(new FV.ValidationResult()); //setup validator to return a no error validation result

            //act
            var controllerResponse = _processImageController.PostProcessImage(request);
            var result = (StatusCodeResult)controllerResponse; //not 'ObjectResult' because there's no object contained in this response

            //assert
            Assert.IsInstanceOf<NoContentResult>(result);
            Assert.AreEqual(expectedStatusCode, result.StatusCode);
        }

        [Test]
        public void given_any_request_when_postProcessImage_controller_method_is_called_then_it_calls_the_validator_with_that_request_object()
        {
            //arrange
            var request = new PostProcessImageRequest();
            _mockPostValidator.Setup(x => x.Validate(request)).Returns(new FV.ValidationResult()); //setup validator to return a no error validation result

            //act
            _processImageController.PostProcessImage(request);

            //assert
            _mockPostValidator.Verify(v => v.Validate(It.Is<PostProcessImageRequest>(obj => obj == request)), Times.Once);
        }

        [Test]
        public void given_an_invalid_request_when_postProcessImage_controller_method_is_called_then_it_returns_400_BadRequest_result()
        {
            //arrange
            var expectedStatusCode = 400;
            var request = new PostProcessImageRequest(); //an empty request will be invalid

            int errorCount = _faker.Random.Int(1, 10); //simulate from 1 to 10 validation errors (triangulation).
            var validationErrorList = new List<ValidationFailure>(); //this list will be used as constructor argument for 'ValidationResult'.
            for (int i = errorCount; i > 0; i--) { validationErrorList.Add(new ValidationFailure(_faker.Random.Word(), _faker.Random.Word())); } //generate from 1 to 10 fake validation errors. Single line for-loop so that it wouldn't distract from what's key in this test.

            var fakeValidationResult = new FV.ValidationResult(validationErrorList); //Need to create ValidationResult so that I could setup Validator mock to return it upon '.Validate()' call. Also this is the only place where it's possible to manipulate the validation result - You can only make the validation result invalid by inserting a list of validation errors as a parameter through a constructor. The boolean '.IsValid' comes from expression 'IsValid => Errors.Count == 0;', so it can't be set manually.
            _mockPostValidator.Setup(v => v.Validate(It.IsAny<PostProcessImageRequest>())).Returns(fakeValidationResult);

            //act
            var controllerResponse = _processImageController.PostProcessImage(request);
            var result = controllerResponse as ObjectResult;

            //assert
            Assert.NotNull(controllerResponse);
            Assert.NotNull(result);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.AreEqual(expectedStatusCode, result.StatusCode);
        }

        [Test]
        public void given_an_invalid_request_when_postProcessImage_controller_method_is_called_then_the_response_BadRequest_result_contains_correct_error_messages()
        {
            //arrange
            var expectedStatusCode = 400;
            var request = new PostProcessImageRequest(); //an empty request will be invalid

            int errorCount = _faker.Random.Int(1, 10); //simulate from 1 to 10 validation errors (triangulation).
            var validationErrorList = new List<ValidationFailure>(); //this list will be used as constructor argument for 'ValidationResult'.
            for (int i = errorCount; i > 0; i--) { validationErrorList.Add(new ValidationFailure(_faker.Random.Word(), _faker.Random.Word())); } //generate from 1 to 10 fake validation errors. Single line for-loop so that it wouldn't distract from what's key in this test.

            var fakeValidationResult = new FV.ValidationResult(validationErrorList); //Need to create ValidationResult so that I could setup Validator mock to return it upon '.Validate()' call. Also this is the only place where it's possible to manipulate the validation result - You can only make the validation result invalid by inserting a list of validation errors as a parameter through a constructor. The boolean '.IsValid' comes from expression 'IsValid => Errors.Count == 0;', so it can't be set manually.
            _mockPostValidator.Setup(v => v.Validate(It.IsAny<PostProcessImageRequest>())).Returns(fakeValidationResult);

            var expectedControllerResponse = new BadRequestObjectResult(validationErrorList); // build up expected controller response to check if the contents of the errors match - that's probably the easiest way to check that.

            //act
            var controllerResponse = _processImageController.PostProcessImage(request);
            var result = controllerResponse as ObjectResult;
            var resultContents = (IList<ValidationFailure>)result.Value;

            //assert
            Assert.NotNull(controllerResponse);
            Assert.NotNull(result);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);

            Assert.IsInstanceOf<IList<ValidationFailure>>(resultContents);
            Assert.NotNull(resultContents);
            Assert.AreEqual(errorCount, resultContents.Count); // there should be 1 validation failure coming from the invalid guid
            Assert.AreEqual(JsonConvert.SerializeObject(expectedControllerResponse), JsonConvert.SerializeObject(controllerResponse)); //expectedControllerResponse
        }

        [Test]
        public void given_an_exception_thrown_check_that_the_controller_throws_correct_exception_and_500_status_code()
        {
            var expectedStatusCode = 500;
            var request = new PostProcessImageRequest(); //an empty request will be invalid

            var fakeValidationResult = new FV.ValidationResult(); //Need to create ValidationResult so that I could setup Validator mock to return it upon '.Validate()' call. Also this is the only place where it's possible to manipulate the validation result - You can only make the validation result invalid by inserting a list of validation errors as a parameter through a constructor. The boolean '.IsValid' comes from expression 'IsValid => Errors.Count == 0;', so it can't be set manually.
            _mockPostValidator.Setup(v => v.Validate(It.IsAny<PostProcessImageRequest>())).Returns(fakeValidationResult);
            _mockUsecase.Setup(x => x.ExecutePost(request)).Throws<ImageNotInsertedToS3>();
            //act
            var controllerResponse = _processImageController.PostProcessImage(request);
            var result = controllerResponse as ObjectResult;

            //assert
            Assert.NotNull(controllerResponse);
            Assert.NotNull(result);
            Assert.AreEqual(expectedStatusCode, result.StatusCode);
        }
        #endregion

        #region Get Process Image

        [Test]
        public void given_a_valid_request_when_GetProcessImage_controller_method_is_called_then_it_returns_a_200_Ok_response()
        {
            //arrange
            var request = MatProcessDataHelper.CreateGetProcessImageRequestObject();
            var expectedStatusCode = 200;
            _mockGetValidator.Setup(x => x.Validate(It.IsAny<GetProcessImageRequest>())).Returns(new FV.ValidationResult()); //setup validator to return a no error validation result

            //act
            var controllerResponse = _processImageController.GetProcessImage(request);
            var result = controllerResponse as ObjectResult;

            //assert
            Assert.NotNull(controllerResponse);
            Assert.NotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(expectedStatusCode, result.StatusCode);
        }

        [Test]
        public void given_a_valid_request_when_GetProcessImage_controller_method_is_called_then_it_returns_an_Ok_result_with_correct_data()
        {
            //arrange
            var request = MatProcessDataHelper.CreateGetProcessImageRequestObject();
            var expectedResponse = new GetProcessImageResponse(_faker.Random.Hash().ToString(), DateTime.Now, request);
            _mockUsecase.Setup(u => u.ExecuteGet(It.IsAny<GetProcessImageRequest>())).Returns(expectedResponse);
            _mockGetValidator.Setup(x => x.Validate(It.IsAny<GetProcessImageRequest>())).Returns(new FV.ValidationResult()); //setup validator to return a no error validation result

            //act
            var controllerResponse = _processImageController.GetProcessImage(request);
            var result = controllerResponse as ObjectResult;
            var resultContents = result.Value as GetProcessImageResponse;

            //assert
            Assert.NotNull(controllerResponse);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.NotNull(result);
            Assert.IsInstanceOf<GetProcessImageResponse>(resultContents);
            Assert.NotNull(resultContents);

            Assert.AreEqual(expectedResponse.Base64Image, resultContents.Base64Image);
            Assert.AreEqual(expectedResponse.GeneratedAt, resultContents.GeneratedAt);
            Assert.AreEqual(expectedResponse.Request.processRef, resultContents.Request.processRef);
            Assert.AreEqual(expectedResponse.Request.imageId, resultContents.Request.imageId);
        }

        [Test]
        public void given_an_invalid_request_when_GetProcessImage_controller_method_is_called_then_it_returns_400_BadRequest_result()
        {
            //arrange
            var expectedStatusCode = 400;
            var request = new GetProcessImageRequest(); //an empty request will be invalid

            int errorCount = _faker.Random.Int(1, 10); //simulate from 1 to 10 validation errors (triangulation).
            var validationErrorList = new List<ValidationFailure>(); //this list will be used as constructor argument for 'ValidationResult'.
            for (int i = errorCount; i > 0; i--) { validationErrorList.Add(new ValidationFailure(_faker.Random.Word(), _faker.Random.Word())); } //generate from 1 to 10 fake validation errors. Single line for-loop so that it wouldn't distract from what's key in this test.

            var fakeValidationResult = new FV.ValidationResult(validationErrorList); //Need to create ValidationResult so that I could setup Validator mock to return it upon '.Validate()' call. Also this is the only place where it's possible to manipulate the validation result - You can only make the validation result invalid by inserting a list of validation errors as a parameter through a constructor. The boolean '.IsValid' comes from expression 'IsValid => Errors.Count == 0;', so it can't be set manually.
            _mockGetValidator.Setup(v => v.Validate(It.IsAny<GetProcessImageRequest>())).Returns(fakeValidationResult);

            //act
            var controllerResponse = _processImageController.GetProcessImage(request);
            var result = controllerResponse as ObjectResult;

            //assert
            Assert.NotNull(controllerResponse);
            Assert.NotNull(result);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.AreEqual(expectedStatusCode, result.StatusCode);
        }

        [Test]
        public void given_an_invalid_request_when_GetProcessImage_controller_method_is_called_then_the_response_BadRequest_result_contains_correct_error_messages()
        {
            //arrange
            var expectedStatusCode = 400;
            var request = new GetProcessImageRequest(); //an empty request will be invalid

            int errorCount = _faker.Random.Int(1, 10); //simulate from 1 to 10 validation errors (triangulation).
            var validationErrorList = new List<ValidationFailure>(); //this list will be used as constructor argument for 'ValidationResult'.
            for (int i = errorCount; i > 0; i--) { validationErrorList.Add(new ValidationFailure(_faker.Random.Word(), _faker.Random.Word())); } //generate from 1 to 10 fake validation errors. Single line for-loop so that it wouldn't distract from what's key in this test.

            var fakeValidationResult = new FV.ValidationResult(validationErrorList); //Need to create ValidationResult so that I could setup Validator mock to return it upon '.Validate()' call. Also this is the only place where it's possible to manipulate the validation result - You can only make the validation result invalid by inserting a list of validation errors as a parameter through a constructor. The boolean '.IsValid' comes from expression 'IsValid => Errors.Count == 0;', so it can't be set manually.
            _mockGetValidator.Setup(v => v.Validate(It.IsAny<GetProcessImageRequest>())).Returns(fakeValidationResult);

            var expectedControllerResponse = new BadRequestObjectResult(validationErrorList); // build up expected controller response to check if the contents of the errors match - that's probably the easiest way to check that.

            //act
            var controllerResponse = _processImageController.GetProcessImage(request);
            var result = controllerResponse as ObjectResult;
            var resultContents = (IList<ValidationFailure>)result.Value;

            //assert
            Assert.NotNull(controllerResponse);
            Assert.NotNull(result);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);

            Assert.IsInstanceOf<IList<ValidationFailure>>(resultContents);
            Assert.NotNull(resultContents);
            Assert.AreEqual(errorCount, resultContents.Count); // there should be 1 validation failure coming from the invalid guid
            Assert.AreEqual(JsonConvert.SerializeObject(expectedControllerResponse), JsonConvert.SerializeObject(controllerResponse)); //expectedControllerResponse
        }

        [Test]
        public void given_an_unexpected_error_would_be_thrown_when_GetProcessImage_controller_method_is_called_then_it_returns_500_status_code()
        {
            //arrange
            int randomErrorNumber = _faker.Random.Int(); //triangulating unexpected exceptions
            string messageRandomizer = _faker.Random.Hash().ToString(); //triangulating error messages

            _mockUsecase.Setup(x => x.ExecuteGet(It.IsAny<GetProcessImageRequest>()))
                .Returns(() => {
                    ErrorThrowerHelper.GenerateError(randomErrorNumber, messageRandomizer); // throw that same random test error
                    return new GetProcessImageResponse(null, DateTime.MinValue, null); //dummy return that will never happen due to error being thrown prior to it. It's only needed to keep compiler happy.
                });

            var request = MatProcessDataHelper.CreateGetProcessImageRequestObject();
            _mockGetValidator.Setup(x => x.Validate(It.IsAny<GetProcessImageRequest>())).Returns(new FV.ValidationResult()); //validation successful

            //act
            var controllerResponse = _processImageController.GetProcessImage(request);
            var result = controllerResponse as ObjectResult;

            //assert
            Assert.NotNull(controllerResponse);
            Assert.NotNull(result);
            Assert.AreEqual(500, result.StatusCode);
        }

        [Test]
        public void given_an_unexpected_error_would_be_thrown_when_GetProcessImage_controller_method_is_called_then_it_returns_a_correct_error_message()
        {
            //arrange
            string expectedErrorMessage = "An error has occured while processing the request - ";

            var randomErrorNumber = _faker.Random.Int(); //triangulating unexpected exceptions
            string messageRandomizer = _faker.Random.Hash().ToString(); //triangulating error messages

            try // throw random error
            {
                ErrorThrowerHelper.GenerateError(randomErrorNumber, messageRandomizer);
            }
            catch (Exception ex) //catch the expected error message
            {
                expectedErrorMessage += (ex.Message + " " + ex.InnerException);
            }

            _mockUsecase.Setup(x => x.ExecuteGet(It.IsAny<GetProcessImageRequest>()))
                .Returns(() => {
                    ErrorThrowerHelper.GenerateError(randomErrorNumber, messageRandomizer); // throw that same random test error
                    return new GetProcessImageResponse(null, DateTime.MinValue, null); //dummy return that will never happen due to error being thrown prior to it. It's only needed to keep compiler happy.
                });

            var request = MatProcessDataHelper.CreateGetProcessImageRequestObject();
            _mockGetValidator.Setup(x => x.Validate(It.IsAny<GetProcessImageRequest>())).Returns(new FV.ValidationResult()); //validation successful

            //act
            var controllerResponse = _processImageController.GetProcessImage(request);
            var result = controllerResponse as ObjectResult;
            var resultContent = result.Value as string; //unwrap the error message from controller response.

            //assert
            Assert.NotNull(controllerResponse);
            Assert.NotNull(result);
            Assert.NotNull(resultContent);
            Assert.AreEqual(expectedErrorMessage, result.Value); //assert if the error message matches what controller wrapped up
        }

        [Test]
        public void given_a_valid_request_when_GetProcessImage_controller_method_is_called_then_it_calls_usecase()
        {
            //arrange
            GetProcessImageRequest request = MatProcessDataHelper.CreateGetProcessImageRequestObject();
            _mockGetValidator.Setup(x => x.Validate(It.IsAny<GetProcessImageRequest>())).Returns(new FV.ValidationResult()); //setup validator to return a no error validation result

            //act
            _processImageController.GetProcessImage(request);

            //assert
            _mockUsecase.Verify(u => u.ExecuteGet(It.IsAny<GetProcessImageRequest>()), Times.Once);
        }

        [Test]
        public void given_a_valid_request_when_GetProcessImage_controller_method_is_called_then_it_calls_usecase_with_correct_data()
        {
            //arrange
            GetProcessImageRequest request = MatProcessDataHelper.CreateGetProcessImageRequestObject();
            _mockGetValidator.Setup(x => x.Validate(It.IsAny<GetProcessImageRequest>())).Returns(new FV.ValidationResult()); //setup validator to return a no error validation result

            //act
            _processImageController.GetProcessImage(request);

            //assert
            _mockUsecase.Verify(u => u.ExecuteGet(It.Is<GetProcessImageRequest>(obj =>
                obj.processRef == request.processRef &&
                obj.imageId == request.imageId
                )), Times.Once);
        }

        [Test]
        public void given_a_request_when_GetProcessImage_controller_method_is_called_then_it_calls_the_validator_with_that_request_object()
        {
            //arrange
            var request = MatProcessDataHelper.CreateGetProcessImageRequestObject();
           
            _mockGetValidator.Setup(x => x.Validate(It.IsAny<GetProcessImageRequest>())).Returns(new FV.ValidationResult()); //setup validator to return a no error validation result

            //act
            _processImageController.GetProcessImage(request);

            //assert
            _mockGetValidator.Verify(v => v.Validate(It.Is<GetProcessImageRequest>(obj => obj == request)), Times.Once);
        }

        [Test]
        public void when_GetProcessImage_controller_method_is_called_then_it_calls_the_logger()
        {
            //arrange
            var request = MatProcessDataHelper.CreateGetProcessImageRequestObject();
            _mockGetValidator.Setup(l => l.Validate(It.IsAny<GetProcessImageRequest>())).Returns(new FV.ValidationResult()); //setup validator to return a no error validation result

            //act
            _processImageController.GetProcessImage(request);

            //assert
            _mockLogger.Verify(l => l.Log(
                        LogLevel.Information,
                        It.IsAny<EventId>(),
                        It.IsAny<FormattedLogValues>(),
                        It.IsAny<Exception>(),
                        It.IsAny<Func<object, Exception, string>>()
                    ), Times.AtLeastOnce);
        }

        [Test]
        public void given_any_request_when_GetProcessImage_controller_method_is_called_then_it_makes_the_logger_log_that_request_happened()
        {
            //arrange
            var request = MatProcessDataHelper.CreateGetProcessImageRequestObject();
            string expectedLogMessage = $"Get ProcessImage request for process reference: {request.processRef} and image Id: {request.imageId}";

            _mockGetValidator.Setup(l => l.Validate(It.IsAny<GetProcessImageRequest>())).Returns(new FV.ValidationResult()); //setup validator to return a no error validation result

            //act
            _processImageController.GetProcessImage(request);

            //assert
            _mockLogger.Verify(l => l.Log(
                        LogLevel.Information,
                        It.IsAny<EventId>(),
                        It.Is<FormattedLogValues>(v => v.ToString().Contains(expectedLogMessage)),
                        It.IsAny<Exception>(),
                        It.IsAny<Func<object, Exception, string>>()
                    ), Times.AtLeastOnce);
        }

        [Test]
        public void given_invalid_request_when_GetProcessImage_controller_method_is_called_then_it_makes_the_logger_log_the_correct_validation_failure_message()
        {
            //arrange
            var request = MatProcessDataHelper.CreateGetProcessImageRequestObject(); //doesn't matter that it's valid right now, because validation failure is built up manually anyway. This object is needs values set so that logger message could be checked.

            int errorCount = _faker.Random.Int(1, 10); //simulate from 1 to 10 validation errors (triangulation).
            var validationErrorList = new List<ValidationFailure>(); //this list will be used as constructor argument for 'ValidationResult'.
            for (int i = errorCount; i > 0; i--) { validationErrorList.Add(new ValidationFailure(_faker.Random.Word(), _faker.Random.Word())); } //generate from 1 to 10 fake validation errors. Single line for-loop so that it wouldn't distract from what's key in this test.

            var fakeValidationResult = new FV.ValidationResult(validationErrorList); //Need to create ValidationResult so that I could setup Validator mock to return it upon '.Validate()' call. Also this is the only place where it's possible to manipulate the validation result - You can only make the validation result invalid by inserting a list of validation errors as a parameter through a constructor. The boolean '.IsValid' comes from expression 'IsValid => Errors.Count == 0;', so it can't be set manually.
            _mockGetValidator.Setup(v => v.Validate(It.IsAny<GetProcessImageRequest>())).Returns(fakeValidationResult);

            string expectedValidationErrorMessages = fakeValidationResult.Errors
                .Select(e => $"Validation error for: '{e.PropertyName}', message: '{e.ErrorMessage}'.")
                .Aggregate((acc, m) => acc + "\n" + m);
            string expectedLogMessage = $"The Get ProcessImage request with process reference: {request.processRef} and image Id: {request.imageId} did not pass the validation:\n\n{expectedValidationErrorMessages}";

            //act
            _processImageController.GetProcessImage(request);

            //assert
            _mockLogger.Verify(l => l.Log(
                        LogLevel.Information,
                        It.IsAny<EventId>(),
                        It.Is<FormattedLogValues>(v => v.ToString().Contains(expectedLogMessage)),
                        It.IsAny<Exception>(),
                        It.IsAny<Func<object, Exception, string>>()
                    ), Times.Once);
        }

        [Test]
        public void given_a_request_with_null_properties_when_GetProcessImage_controller_method_is_called_then_it_makes_the_logger_display_those_null_properties_as_string_saying_null() //otherwise it would just display empty
        {
            //arrange
            var request = new GetProcessImageRequest(); // both properties are 'null' now.

            //need the validation failure here, because the implementation would only fire off upon validation failure.
            var validationErrorList = new List<ValidationFailure>();
            validationErrorList.Add(new ValidationFailure(_faker.Random.Word(), _faker.Random.Word()));

            var fakeValidationResult = new FV.ValidationResult(validationErrorList); //Need to create ValidationResult so that I could setup Validator mock to return it upon '.Validate()' call. Also this is the only place where it's possible to manipulate the validation result - You can only make the validation result invalid by inserting a list of validation errors as a parameter through a constructor. The boolean '.IsValid' comes from expression 'IsValid => Errors.Count == 0;', so it can't be set manually.
            _mockGetValidator.Setup(v => v.Validate(It.IsAny<GetProcessImageRequest>())).Returns(fakeValidationResult);

            string expectedLogMessageSubstring = $"The Get ProcessImage request with process reference: {"null"} and image Id: {"null"} did not pass the validation:"; //set up only substring, because the test only cares about this first part of the full log message that would be passed in.

            //act
            _processImageController.GetProcessImage(request);
            
            //assert
            _mockLogger.Verify(l => l.Log(
                        LogLevel.Information,
                        It.IsAny<EventId>(),
                        It.Is<FormattedLogValues>(v => v.ToString().Contains(expectedLogMessageSubstring)),
                        It.IsAny<Exception>(),
                        It.IsAny<Func<object, Exception, string>>()
                    ), Times.AtLeastOnce);
        }

        //Do we need to log the exception messages with the logger in the controller? Or does our logging service pick out a message from response? If no, need another test.

        #endregion
    }
}
