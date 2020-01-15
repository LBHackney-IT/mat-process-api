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

namespace mat_process_api.Tests.V1.Controllers
{
    [TestFixture]
    public class ProcessDataImagesControllerTests
    {
        private ProcessImageController _processImageController;
        private Mock<IProcessImageUseCase> _mockUsecase;
        private Mock<IPostProcessImageRequestValidator> _mockPostValidator;
        private Faker _faker = new Faker();

        [SetUp]
        public void SetUp()
        {
            _mockUsecase = new Mock<IProcessImageUseCase>();
            _mockPostValidator = new Mock<IPostProcessImageRequestValidator>();
            _processImageController = new ProcessImageController(_mockUsecase.Object, _mockPostValidator.Object);
        }

        [Test]
        public void given_valid_request_when_postProcessImage_controller_method_is_called_then_usecase_is_called()
        {
            //arrange
            var request = new PostProcessImageRequest();

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

            //act
            _processImageController.PostProcessImage(request);

            //assert
            _mockPostValidator.Verify(v => v.Validate(It.Is<PostProcessImageRequest>(obj => obj == request)), Times.Once);
        }
    }
}
