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
using System.Threading.Tasks;

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

        #region Post Process Image

        [Test]
        public void when_ProcessImageUseCase_ExecutePost_method_is_called_then_it_calls_the_gateway()
        {
            //arrange
            var request = MatProcessDataHelper.CreatePostProcessImageRequestObject();
            _mockImageDecoder.Setup(x => x.DecodeBase64ImageString(request.base64Image)).ReturnsAsync(new Base64DecodedData() { imageExtension = _faker.Random.Word() });
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
            _mockImageDecoder.Setup(x => x.DecodeBase64ImageString(request.base64Image)).ReturnsAsync(new Base64DecodedData() { imageExtension = _faker.Random.Word() });
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

            _mockImageDecoder.Setup(d => d.DecodeBase64ImageString(It.IsAny<string>())).ReturnsAsync(decodedData); //At this step, this test does not care about the validity of the decoder output, it only cares to see if the usecase passes the decoder output into gateway or not.

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

        #endregion

        #region Get Process Image

        [Test]
        public void when_ProcessImageUseCase_ExecuteGet_method_is_called_then_it_calls_the_gateway()
        {
            //act
            _processImageUseCase.ExecuteGet(new GetProcessImageRequest()); // no need for arrange, since it does not matter how the request object got set up for this test.

            //assert
            _mockGateway.Verify(g => g.RetrieveImage(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task given_a_request_when_ProcessImageUseCase_ExecuteGet_method_is_called_then_it_calls_the_gateway_with_imageKey_generated_based_on_the_request()
        {
            //assert
            var request = MatProcessDataHelper.CreateGetProcessImageRequestObject();
            var imageKey = await ImagePersistingHelper.generateImageKey(request.processType, request.imageId, request.processRef, request.fileExtension);

            //act
            _processImageUseCase.ExecuteGet(request);

            //assert
            _mockGateway.Verify(g => g.RetrieveImage(It.Is<string>(obj => obj == imageKey)), Times.Once);
        }

        [Test]
        public async Task given_a_request_when_ProcessImageUseCase_ExecuteGet_method_is_called_then_it_returns_the_response_object_of_correct_type_and_data()
        {
            //assert
            var request = MatProcessDataHelper.CreateGetProcessImageRequestObject();
            var imageKey = await ImagePersistingHelper.generateImageKey(request.processType, request.imageId, request.processRef, request.fileExtension);

            var expectedBase64ImageString = MatProcessDataHelper.CreatePostProcessImageRequestObject().base64Image;
            _mockGateway.Setup(g => g.RetrieveImage(It.Is<string>(obj => obj == imageKey))).Returns(expectedBase64ImageString);

            var expectedUsecaseResponse = new GetProcessImageResponse(expectedBase64ImageString, DateTime.Now, request); //The date time is impossible to test equality for, as it will differ in few microseconds from the actual response one. So I set it to DateTime.Min to signify its unimportance.

            //act
            var usecaseResponse = _processImageUseCase.ExecuteGet(request);

            //assert
            Assert.IsNotNull(usecaseResponse);
            Assert.IsInstanceOf<GetProcessImageResponse>(usecaseResponse);

            Assert.AreEqual(expectedUsecaseResponse.Base64Image, usecaseResponse.Base64Image);
            Assert.AreEqual(expectedUsecaseResponse.Request.processRef, usecaseResponse.Request.processRef);
            Assert.AreEqual(expectedUsecaseResponse.Request.imageId, usecaseResponse.Request.imageId);

            //This assertion has an accuracy of 1 second. If time difference between [when expected object was created] and [when actual usecase response was created] is within 1 second, then the times are considered equal. 1 Second is plenty of time for the code in between to run, considering it's using a Mock gateway.
            Assert.Less((usecaseResponse.GeneratedAt - expectedUsecaseResponse.GeneratedAt).TotalMilliseconds, 1000);
        }

        #endregion
    }
}
