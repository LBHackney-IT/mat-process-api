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
        private Mock<IProcessImageGateway> _mockGateway;
        private Mock<IProcessImageDecoder> _mockImageDecoder;
        private Faker _faker = new Faker();

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<IProcessImageGateway>();
            _mockImageDecoder = new Mock<IProcessImageDecoder>();
            _processImageUseCase = new ProcessImageUseCase(_mockGateway.Object, _mockImageDecoder.Object);
        }

        [Test]
        public void when_ProcessImageUseCase_ExecutePost_method_is_called_then_it_calls_the_gateway()
        {
            //arrange
            var request = MatProcessDataHelper.CreatePostProcessImageRequestObject();

            //act
            _processImageUseCase.ExecutePost(request);

            //assert
            _mockGateway.Verify(g => g.PostProcessImage(It.IsAny<ProcessImageData>()), Times.Once);
        }

        [Test]
        public void given_a_request_when_ProcessImageUseCase_ExecutePost_method_is_called_then_it_calls_the_ProcessImageDecoder_with_imageString_from_request()
        {
            //arrange
            var request = MatProcessDataHelper.CreatePostProcessImageRequestObject();

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
            _mockGateway.Verify(g => g.PostProcessImage(It.Is<ProcessImageData>(obj =>
                obj.processRef == request.processRef &&
                obj.imageId == request.imageId &&
                obj.imageData.imageBytes.SequenceEqual(decodedData.imageBytes) &&
                obj.imageData.imageType == decodedData.imageType &&
                obj.imageData.imageExtension == decodedData.imageExtension
                )), Times.Once);
        }
    }
}
