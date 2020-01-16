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


namespace mat_process_api.Tests.V1.Controllers
{
    [TestFixture]
    public class ProcessDataImagesControllerTests
    {
        private ProcessImageController _processImageController;
        private Mock<IProcessImageUseCase> _mockUsecase;
        private Mock<IPostProcessImageRequestValidator> _mockPostValidator;
        private Mock<IGetProcessImageRequestValidator> _mockGetValidator;
        private Faker _faker = new Faker();

        [SetUp]
        public void SetUp()
        {
            _mockUsecase = new Mock<IProcessImageUseCase>();
            _mockPostValidator = new Mock<IPostProcessImageRequestValidator>();
            _mockGetValidator = new Mock<IGetProcessImageRequestValidator>();
            _processImageController = new ProcessImageController(_mockUsecase.Object, _mockPostValidator.Object, _mockGetValidator.Object);
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

        #endregion

        #region Get Process Image

        [Test]
        public void given_a_valid_request_when_GetProcessImage_controller_method_is_called_then_it_returns_a_200_Ok_response()
        {
            //arrange
            var request = new GetProcessImageRequest();
            var expectedStatusCode = 200;
            _mockGetValidator.Setup(x => x.Validate(request)).Returns(new FV.ValidationResult()); //setup validator to return a no error validation result

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
            _mockUsecase.Setup(u => u.ExecuteGet(It.Is<GetProcessImageRequest>(obj => obj == request))).Returns(expectedResponse);
            _mockGetValidator.Setup(x => x.Validate(request)).Returns(new FV.ValidationResult()); //setup validator to return a no error validation result

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
            var request = new GetProcessImageRequest();
            _mockGetValidator.Setup(x => x.Validate(request)).Returns(new FV.ValidationResult()); //validation successful

            _mockUsecase.Setup(x => x.ExecuteGet(It.IsAny<GetProcessImageRequest>())).Throws<AggregateException>();

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

            void GenerateRandomError(int errorNumber)
            {
                switch (errorNumber)
                {
                    case 0: throw new OutOfMemoryException();
                    case 1: throw new IndexOutOfRangeException();
                    case 2: throw new ArgumentNullException();
                    default: throw new AggregateException();
                }
            }

            var randomErrorNo = _faker.Random.Int(0, 3); //triangulating unexpected exceptions

            try // throw random error
            {
                GenerateRandomError(randomErrorNo);
            }
            catch (Exception ex) //catch the expected error message
            {
                expectedErrorMessage += (ex.Message + " " + ex.InnerException);
            }

            _mockUsecase.Setup(x => x.ExecuteGet(It.IsAny<GetProcessImageRequest>()))
                .Returns(() => {
                    GenerateRandomError(randomErrorNo); // throw that same random test error
                    return new GetProcessImageResponse(null, DateTime.MinValue, null); //dummy return that will never happen due to error being thrown prior to it. It's only needed to keep compiler happy.
                });

            var request = new GetProcessImageRequest();
            _mockGetValidator.Setup(x => x.Validate(request)).Returns(new FV.ValidationResult()); //validation successful

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
            var request = new GetProcessImageRequest();
            _mockGetValidator.Setup(x => x.Validate(request)).Returns(new FV.ValidationResult()); //setup validator to return a no error validation result

            //act
            _processImageController.GetProcessImage(request);

            //assert
            _mockUsecase.Verify(u => u.ExecuteGet(It.IsAny<GetProcessImageRequest>()), Times.Once);
        }

        [Test]
        public void given_a_valid_request_when_GetProcessImage_controller_method_is_called_then_it_calls_usecase_with_correct_data()
        {
            //arrange
            var request = MatProcessDataHelper.CreateGetProcessImageRequestObject();
            _mockGetValidator.Setup(x => x.Validate(request)).Returns(new FV.ValidationResult()); //setup validator to return a no error validation result

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
            var request = new GetProcessImageRequest();
            _mockGetValidator.Setup(x => x.Validate(request)).Returns(new FV.ValidationResult()); //setup validator to return a no error validation result

            //act
            _processImageController.GetProcessImage(request);

            //assert
            _mockGetValidator.Verify(v => v.Validate(It.Is<GetProcessImageRequest>(obj => obj == request)), Times.Once);
        }

        #endregion
    }
}
