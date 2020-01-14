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
            var request = new PostProcessImageRequest();

            //act
            _processImageUseCase.ExecutePost(request);

            //assert
            _mockGateway.Verify(g => g.PostProcessImage(It.IsAny<ProcessImageData>()), Times.Once);
        }

        [Test]
        public void given_a_request_when_ProcessImageUseCase_ExecutePost_method_is_called_then_it_calls_the_ProcessImageDecoder_with_imageString_from_request()
        {
            //arrange
            var request = new PostProcessImageRequest()
            {
                processRef = _faker.Random.Guid().ToString(),
                imageId = _faker.Random.Guid().ToString(),
                base64Image = _faker.Random.Hash()
            };

            //act
            _processImageUseCase.ExecutePost(request);

            //assert
            _mockImageDecoder.Verify(d => d.DecodeBase64ImageString(It.Is<string>(s => s == request.base64Image)), Times.Once);
        }

        [Test]
        public void given_a_request_object_when_ProcessImageUseCase_ExecutePost_method_is_called_then_it_calls_the_gateway_with_correct_data()
        {
            //arrange
            var request = new PostProcessImageRequest()
            {
                processRef = _faker.Random.Guid().ToString(),
                imageId = _faker.Random.Guid().ToString(),
                base64Image = _faker.Random.Hash()
            };

            byte[] expectedImageBytes = Convert.FromBase64String(request.base64Image);

            _mockImageDecoder.Setup(d => d.DecodeBase64ImageString(request.base64Image)).Returns(expectedImageBytes);

            //act
            _processImageUseCase.ExecutePost(request);

            //assert
            _mockGateway.Verify(g => g.PostProcessImage(It.Is<ProcessImageData>(obj =>
                obj.processRef == request.processRef &&
                obj.imageId == request.imageId &&
                obj.imageBytes.SequenceEqual(expectedImageBytes)
                )), Times.Once);
        }
    }
}
