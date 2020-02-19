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

object 

        //[Test]
        //public void given_an_invalid_request_when_postProcessImage_controller_method_is_called_then_it_returns_400_BadRequest_result()
        //{
        //    //arrange
        //    var expectedStatusCode = 400;
        //    var request = new PostProcessImageRequest(); //an empty request will be invalid

        //    int errorCount = _faker.Random.Int(1, 10); //simulate from 1 to 10 validation errors (triangulation).
        //    var validationErrorList = new List<ValidationFailure>(); //this list will be used as constructor argument for 'ValidationResult'.
        //    for (int i = errorCount; i > 0; i--) { validationErrorList.Add(new ValidationFailure(_faker.Random.Word(), _faker.Random.Word())); } //generate from 1 to 10 fake validation errors. Single line for-loop so that it wouldn't distract from what's key in this test.

        //    var fakeValidationResult = new FV.ValidationResult(validationErrorList); //Need to create ValidationResult so that I could setup Validator mock to return it upon '.Validate()' call. Also this is the only place where it's possible to manipulate the validation result - You can only make the validation result invalid by inserting a list of validation errors as a parameter through a constructor. The boolean '.IsValid' comes from expression 'IsValid => Errors.Count == 0;', so it can't be set manually.
        //    _mockPostValidator.Setup(v => v.Validate(It.IsAny<PostProcessImageRequest>())).Returns(fakeValidationResult);

        //    //act
        //    var controllerResponse = _processImageController.PostProcessImage(request);
        //    var result = controllerResponse as ObjectResult;

        //    //assert
        //    Assert.NotNull(controllerResponse);
        //    Assert.NotNull(result);
        //    Assert.IsInstanceOf<BadRequestObjectResult>(result);
        //    Assert.AreEqual(expectedStatusCode, result.StatusCode);
        //}

        //[Test]
        //public void given_an_invalid_request_when_postProcessImage_controller_method_is_called_then_the_response_BadRequest_result_contains_correct_error_messages()
        //{
        //    //arrange
        //    var expectedStatusCode = 400;
        //    var request = new PostProcessImageRequest(); //an empty request will be invalid

        //    int errorCount = _faker.Random.Int(1, 10); //simulate from 1 to 10 validation errors (triangulation).
        //    var validationErrorList = new List<ValidationFailure>(); //this list will be used as constructor argument for 'ValidationResult'.
        //    for (int i = errorCount; i > 0; i--) { validationErrorList.Add(new ValidationFailure(_faker.Random.Word(), _faker.Random.Word())); } //generate from 1 to 10 fake validation errors. Single line for-loop so that it wouldn't distract from what's key in this test.

        //    var fakeValidationResult = new FV.ValidationResult(validationErrorList); //Need to create ValidationResult so that I could setup Validator mock to return it upon '.Validate()' call. Also this is the only place where it's possible to manipulate the validation result - You can only make the validation result invalid by inserting a list of validation errors as a parameter through a constructor. The boolean '.IsValid' comes from expression 'IsValid => Errors.Count == 0;', so it can't be set manually.
        //    _mockPostValidator.Setup(v => v.Validate(It.IsAny<PostProcessImageRequest>())).Returns(fakeValidationResult);

        //    var expectedControllerResponse = new BadRequestObjectResult(validationErrorList); // build up expected controller response to check if the contents of the errors match - that's probably the easiest way to check that.

        //    //act
        //    var controllerResponse = _processImageController.PostProcessImage(request);
        //    var result = controllerResponse as ObjectResult;
        //    var resultContents = (IList<ValidationFailure>)result.Value;

        //    //assert
        //    Assert.NotNull(controllerResponse);
        //    Assert.NotNull(result);
        //    Assert.IsInstanceOf<BadRequestObjectResult>(result);

        //    Assert.IsInstanceOf<IList<ValidationFailure>>(resultContents);
        //    Assert.NotNull(resultContents);
        //    Assert.AreEqual(errorCount, resultContents.Count); // there should be 1 validation failure coming from the invalid guid
        //    Assert.AreEqual(JsonConvert.SerializeObject(expectedControllerResponse), JsonConvert.SerializeObject(controllerResponse)); //expectedControllerResponse
        //}

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
        public void given_successful_request_validation_when_GetProcessImage_controller_method_is_called_then_it_returns_a_200_Ok_response()
        {
            //arrange
            var expectedStatusCode = 200;
            _mockGetValidator.Setup(x => x.Validate(It.IsAny<GetProcessImageRequest>())).Returns(new FV.ValidationResult()); //setup validator to return a no error validation result

            //act
            var controllerResponse = _processImageController.GetProcessImage(new GetProcessImageRequest());
            var result = controllerResponse as ObjectResult;

            //assert
            Assert.NotNull(controllerResponse);
            Assert.NotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(expectedStatusCode, result.StatusCode);
        }

        [Test]
        public void given_successful_request_validation_when_GetProcessImage_controller_method_is_called_then_it_returns_an_Ok_result_with_correct_data_based_of_that_request()
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
        public void given_failed_request_validation_when_GetProcessImage_controller_method_is_called_then_it_returns_400_BadRequest_result()
        {
            //arrange
            var expectedStatusCode = 400;
            int errorCount = _faker.Random.Int(1, 10); //simulate from 1 to 10 validation errors (triangulation).
            var validationErrorList = new List<ValidationFailure>(); //this list will be used as constructor argument for 'ValidationResult'.
            for (int i = errorCount; i > 0; i--) { validationErrorList.Add(new ValidationFailure(_faker.Random.Word(), _faker.Random.Word())); } //generate from 1 to 10 fake validation errors. Single line for-loop so that it wouldn't distract from what's key in this test.

            var fakeValidationResult = new FV.ValidationResult(validationErrorList); //Need to create ValidationResult so that I could setup Validator mock to return it upon '.Validate()' call. Also this is the only place where it's possible to manipulate the validation result - You can only make the validation result invalid by inserting a list of validation errors as a parameter through a constructor. The boolean '.IsValid' comes from expression 'IsValid => Errors.Count == 0;', so it can't be set manually.
            _mockGetValidator.Setup(v => v.Validate(It.IsAny<GetProcessImageRequest>())).Returns(fakeValidationResult);

            //act
            var controllerResponse = _processImageController.GetProcessImage(new GetProcessImageRequest());
            var result = controllerResponse as ObjectResult;

            //assert
            Assert.NotNull(controllerResponse);
            Assert.NotNull(result);
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.AreEqual(expectedStatusCode, result.StatusCode);
        }

        [Test]
        public void given_failed_request_validation_when_GetProcessImage_controller_method_is_called_then_the_response_BadRequest_result_contains_error_messages_returned_by_validator()
        {
            //arrange
            int errorCount = _faker.Random.Int(1, 10); //simulate from 1 to 10 validation errors (triangulation).
            var validationErrorList = new List<ValidationFailure>(); //this list will be used as constructor argument for 'ValidationResult'.
            for (int i = errorCount; i > 0; i--) { validationErrorList.Add(new ValidationFailure(_faker.Random.Word(), _faker.Random.Word())); } //generate from 1 to 10 fake validation errors. Single line for-loop so that it wouldn't distract from what's key in this test.

            var fakeValidationResult = new FV.ValidationResult(validationErrorList); //Need to create ValidationResult so that I could setup Validator mock to return it upon '.Validate()' call. Also this is the only place where it's possible to manipulate the validation result - You can only make the validation result invalid by inserting a list of validation errors as a parameter through a constructor. The boolean '.IsValid' comes from expression 'IsValid => Errors.Count == 0;', so it can't be set manually.
            _mockGetValidator.Setup(v => v.Validate(It.IsAny<GetProcessImageRequest>())).Returns(fakeValidationResult);

            var expectedControllerResponse = new BadRequestObjectResult(validationErrorList); // build up expected controller response to check if the contents of the errors match - that's probably the easiest way to check that.

            //act
            var controllerResponse = _processImageController.GetProcessImage(new GetProcessImageRequest());
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
            var expectedStatusCode = 500;
            var randomExpectedError = ErrorThrowerHelper.GenerateError();

            _mockUsecase.Setup(x => x.ExecuteGet(It.IsAny<GetProcessImageRequest>())).Throws(randomExpectedError); // throw random test error (triangulation)

            _mockGetValidator.Setup(x => x.Validate(It.IsAny<GetProcessImageRequest>())).Returns(new FV.ValidationResult()); //validation successful

            //act
            var controllerResponse = _processImageController.GetProcessImage(new GetProcessImageRequest());
            var result = controllerResponse as ObjectResult;

            //assert
            Assert.NotNull(controllerResponse);
            Assert.NotNull(result);
            Assert.AreEqual(expectedStatusCode, result.StatusCode);
        }

        [Test]
        public void given_an_unexpected_error_would_be_thrown_when_GetProcessImage_controller_method_is_called_then_it_returns_a_correctly_formatted_error_message_that_is_based_of_exception()
        {
            //arrange
            string expectedErrorMessage = "An error has occured while processing the request - ";

            var randomExpectedError = ErrorThrowerHelper.GenerateError();

            try // throw random error
            {
                throw randomExpectedError;
            }
            catch (Exception ex) //catch the expected error message
            {
                expectedErrorMessage += (ex.Message + " " + ex.InnerException);
            }

            _mockUsecase.Setup(x => x.ExecuteGet(It.IsAny<GetProcessImageRequest>())).Throws(randomExpectedError);

            _mockGetValidator.Setup(x => x.Validate(It.IsAny<GetProcessImageRequest>())).Returns(new FV.ValidationResult()); //validation successful

            //act
            var controllerResponse = _processImageController.GetProcessImage(new GetProcessImageRequest());
            var result = controllerResponse as ObjectResult;
            var resultContent = result.Value as string; //unwrap the error message from controller response.

            //assert
            Assert.NotNull(controllerResponse);
            Assert.NotNull(result);
            Assert.NotNull(resultContent);
            Assert.AreEqual(expectedErrorMessage, result.Value); //assert if the error message matches what controller wrapped up
        }

        [Test]
        public void given_successful_request_validation_when_GetProcessImage_controller_method_is_called_then_it_calls_usecase()
        {
            //arrange
            _mockGetValidator.Setup(x => x.Validate(It.IsAny<GetProcessImageRequest>())).Returns(new FV.ValidationResult()); //setup validator to return a no error validation result

            //act
            _processImageController.GetProcessImage(new GetProcessImageRequest());

            //assert
            _mockUsecase.Verify(u => u.ExecuteGet(It.IsAny<GetProcessImageRequest>()), Times.Once);
        }

        [Test]
        public void given_successful_request_validation_when_GetProcessImage_controller_method_is_called_then_it_calls_usecase_with_correct_data_based_of_request()
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
        public void given_a_request_object_when_GetProcessImage_controller_method_is_called_then_it_calls_the_validator_with_that_request_object()
        {
            //arrange
            var request = MatProcessDataHelper.CreateGetProcessImageRequestObject();
           
            _mockGetValidator.Setup(x => x.Validate(It.IsAny<GetProcessImageRequest>())).Returns(new FV.ValidationResult()); //setup validator to return a no error validation result

            //act
            _processImageController.GetProcessImage(request);

            //assert
            _mockGetValidator.Verify(v => v.Validate(It.Is<GetProcessImageRequest>(obj => obj == request)), Times.Once);
        }

        /// <summary>
        /// The test aims to show that the request gets logged at least once regardless of validator behaviour.
        /// By behaviour it is meant that it shouldn't matter whether the validator has returned true or false, or if it crashed entirely.
        /// If your initial log is dependent on the validator not crashing, then it's a bad of way of ensuring that every request gets logged.
        /// So, this test is set up in such a way, that doesn't have validator setup to show that despite the crash during validation (which happens in the try-catch block), the logger was indeed called at least once.
        /// This also demonstrates pretty clearly that whatever happens in the validator has no effect on the request getting logged.
        /// In NUnit tests fail when either Assertion makes the test crash, or when the code inside it crashes. In order to proceed with assertion that shows that logger call does not depend on validation flow crashing, try-catch was needed.
        /// Same applies for the test bellow this test: `given_a_request_when_GetProcessImage_controller_method_is_called_then_it_makes_the_logger_log_the_information_about_the_request_regardless_of_the_validator_behaviour`
        /// </summary>
        [Test]
        public void when_GetProcessImage_controller_method_is_called_then_it_calls_the_logger_regardless_of_the_validator_behaviour()
        {
            //act
            try { _processImageController.GetProcessImage(new GetProcessImageRequest()); } catch(Exception ex) { } //the empty try-catch is needed to ignore the exception being thrown due to absense of validation result returned later down the line, so that the test could get to the assertion step.

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
        public void given_a_request_when_GetProcessImage_controller_method_is_called_then_it_makes_the_logger_log_the_information_about_the_request_regardless_of_the_validator_behaviour()  //Since there's no validator setup, it returns no validation result, which will make the flow crash. If the Verify bit shows that logger was called under these conditions, then this shows that validation result or behaviour has no influence on logger being called at least once, which is important if you want to register that request happened regardless of the validity of the request.
        {
            //arrange
            var request = MatProcessDataHelper.CreateGetProcessImageRequestObject();
            string expectedLogMessage = $"Get ProcessImage request for Process Type: {request.processType}, Process Reference: {request.processRef}, Image Id: {request.imageId} and File Extension: {request.fileExtension}";

            //act
            try { _processImageController.GetProcessImage(request); } catch(Exception ex) { } //the empty try-catch is needed to ignore the exception being thrown due to absense of validation result returned later down the line. The idea is that logger is the first thing that is being called once request comes in.

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
        public void given_a_request_with_null_properties_when_GetProcessImage_controller_method_is_called_then_it_makes_the_logger_display_those_null_properties_as_string_saying_null_during_the_initial_logger_call_independent_of_the_validator_behaviour()  //Since there's no validator setup, it returns no validation result. If the Verify bit shows that logger was called under these conditions, then this shows that validation result has no influence on logger being called at least once, which is important if you want to register that request happened regardless of the validity of the request.
        {
            //arrange
            var request = new GetProcessImageRequest(); //request with null properties
            string expectedLogMessage = $"Get ProcessImage request for Process Type: {"null"}, Process Reference: {"null"}, Image Id: {"null"} and File Extension: {"null"}";

            //act
            try { _processImageController.GetProcessImage(request); } catch (Exception ex) { } //the empty try-catch is needed to ignore the exception being thrown due to absense of validation result returned later down the line. The idea is that logger is the first thing that is being called once request comes in.

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
        public void given_failed_request_validation_when_GetProcessImage_controller_method_is_called_then_it_makes_the_logger_log_the_correctly_formatted_validation_failure_messages_returned_by_the_validator()
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
            string expectedLogMessage = $"Get ProcessImage request for Process Type: {request.processType}, Process Reference: {request.processRef}, Image Id: {request.imageId} and File Extension: {request.fileExtension} did not pass the validation:\n\n{expectedValidationErrorMessages}";

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

            string expectedLogMessageSubstring = $"Get ProcessImage request for Process Type: {"null"}, Process Reference: {"null"}, Image Id: {"null"} and File Extension: {"null"}"; //set up only substring, because the test only cares about this first part of the full log message that would be passed in.

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


        [Test]
        public void given_an_ImageNotFound_exception_thrown_by_gateway_verify_controller_returns_404_status_code_and_message()
        {
            var expectedStatusCode = 404;
            var request = new GetProcessImageRequest() { imageId = _faker.Random.Guid().ToString()};  //an empty request will be invalid

            var expectedErrorMessage = $"The image with ID = {request.imageId} has not been found.";

            var fakeValidationResult = new FV.ValidationResult(); 
            _mockGetValidator.Setup(v => v.Validate(It.IsAny<GetProcessImageRequest>())).Returns(fakeValidationResult);
            _mockUsecase.Setup(x => x.ExecuteGet(request)).Throws<ImageNotFound>();
            //act
            var controllerResponse = _processImageController.GetProcessImage(request);
            var result = controllerResponse as NotFoundObjectResult;

            //assert
            Assert.NotNull(controllerResponse);
            Assert.NotNull(result);
            Assert.AreEqual(expectedStatusCode, result.StatusCode);
            Assert.AreEqual(expectedErrorMessage, result.Value);
        }
        #endregion
    }
}
