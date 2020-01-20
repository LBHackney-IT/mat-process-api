using System;
using mat_process_api.V1.Boundary;
using mat_process_api.V1.UseCase;
using NUnit.Framework;
using Moq;
using mat_process_api.V1.Gateways;
using Bogus;
using mat_process_api.V1.Domain;
using System.Linq;
using mat_process_api.V1.Helpers;
using mat_process_api.Tests.V1.Helper;

namespace mat_process_api.Tests.V1.UseCase
{
    [TestFixture]
    public class ProcessImageUseCaseTests
    {
        private IProcessImageUseCase _processImageUseCase;
        private Mock<IImagePersistingGateway> _mockGateway;
        private Mock<IProcessImageDecoder> _mockImageDecoder;
        private Faker _faker = new Faker();

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<IImagePersistingGateway>();
            _mockImageDecoder = new Mock<IProcessImageDecoder>();
            _processImageUseCase = new ProcessImageUseCase(_mockGateway.Object, _mockImageDecoder.Object);
        }

        [Test]
        public void when_ProcessImageUseCase_ExecutePost_method_is_called_then_it_calls_the_gateway()
        {
            //arrange
            var request = MatProcessDataHelper.CreatePostProcessImageRequestObject();
            _mockImageDecoder.Setup(x => x.DecodeBase64ImageString(request.base64Image)).Returns(new Base64DecodedData() { imageExtension = _faker.Random.Word() });
            //act
            _processImageUseCase.ExecutePost(request);

            //assert
            _mockGateway.Verify(g => g.UploadImage(It.IsAny<ProcessImageData>()), Times.Once);
        }

        [Test]
        public void given_a_request_when_ProcessImageUseCase_ExecutePost_method_is_called_then_it_calls_the_ProcessImageDecoder_with_imageString_from_request()
        {
            //arrange
            var request = MatProcessDataHelper.CreatePostProcessImageRequestObject();
            _mockImageDecoder.Setup(x => x.DecodeBase64ImageString(request.base64Image)).Returns(new Base64DecodedData() { imageExtension = _faker.Random.Word() });
            //act
            _processImageUseCase.ExecutePost(request);

            //assert
            _mockImageDecoder.Verify(d => d.DecodeBase64ImageString(It.Is<string>(s => s == request.base64Image)), Times.Once);
        }

        [Test]
        public void given_a_request_object_when_ProcessImageUseCase_ExecutePost_method_is_called_then_it_calls_the_gateway_with_correct_data() //test to see if usecase calls gateway with domain correctly built domain object
        {
            //arrange
            var request = MatProcessDataHelper.CreatePostProcessImageRequestObject();
            var decodedData = MatProcessDataHelper.CreateBase64DecodedDataObject();

            _mockImageDecoder.Setup(d => d.DecodeBase64ImageString(It.IsAny<string>())).Returns(decodedData); //At this step, this test does not care about the validity of the decoder output, it only cares to see if the usecase passes the decoder output into gateway or not.

            //act
            _processImageUseCase.ExecutePost(request);

            //assert
            _mockGateway.Verify(g => g.UploadImage(It.Is<ProcessImageData>(obj =>
                obj.processRef == request.processRef &&
                obj.imageId == request.imageId &&
                obj.imageData.imagebase64String.SequenceEqual(decodedData.imagebase64String) &&
                obj.imageData.imageType == decodedData.imageType &&
                obj.imageData.imageExtension == decodedData.imageExtension &&
                obj.key == $"{request.processType}/{request.processRef}/{request.imageId}.{decodedData.imageExtension}"
                )), Times.Once);
        }
    }
}
