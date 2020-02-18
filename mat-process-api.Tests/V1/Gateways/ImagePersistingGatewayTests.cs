using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SecurityToken.Model;
using Bogus;
using mat_process_api.V1.Boundary;
using mat_process_api.V1.Domain;
using mat_process_api.V1.Exceptions;
using mat_process_api.V1.Factories;
using mat_process_api.V1.Gateways;
using mat_process_api.V1.Helpers;
using mat_process_api.V1.Infrastructure;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace mat_process_api.Tests.V1.Gateways
{
    [TestFixture]
    public class ImagePersistingGatewayTests
    {
        private ImagePersistingGateway classUnderTest;
        private Mock<IAwsAssumeRoleHelper> mockAssumeRoleHelper;
        private Mock<IAmazonS3Client> mockS3Client;
        private Faker faker;
        [SetUp]
        public void set_up()
        {
            mockAssumeRoleHelper = new Mock<IAwsAssumeRoleHelper>();
            faker = new Faker();
            mockS3Client = new Mock<IAmazonS3Client>();
            classUnderTest = new ImagePersistingGateway(mockS3Client.Object, mockAssumeRoleHelper.Object);
        }
        [Test]
        public void test_that_gateway_class_implements_gateway_interface()
        {
            Assert.NotNull(classUnderTest is IImagePersistingGateway);
        }

        [Test]
        public void test_that_gateway_returns_does_not_throw_an_exception_when_successfully_inserting_an_image()
        {
            //arrange
            var request = new ProcessImageData() { imageData = new Base64DecodedData() { imagebase64String = faker.Random.Word()}, imageId = faker.Random.Word(),
                processRef = faker.Random.Guid().ToString()};
            var expectedResponse = new PutObjectResponse();
            expectedResponse.HttpStatusCode = System.Net.HttpStatusCode.OK;

            mockS3Client.Setup(x => x.insertImage(It.IsAny<AWSCredentials>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(expectedResponse);
            mockAssumeRoleHelper.Setup(x => x.GetTemporaryCredentials()).ReturnsAsync(It.IsAny<Credentials>());
            //act
            Assert.DoesNotThrow(() => classUnderTest.UploadImage(request));
        }

        [Test]
        public void test_that_gateway_throws_document_not_inserted_when_status_code_is_not_204()
        {
            //arrange
            var request = new ProcessImageData()
            {
                imageData = new Base64DecodedData(),
                imageId = faker.Random.Word(),
                processRef = faker.Random.Guid().ToString()
            };
            var expectedResponse = new PutObjectResponse();
            expectedResponse.HttpStatusCode = HttpStatusCode.Conflict;

            mockS3Client.Setup(x => x.insertImage(It.IsAny<AWSCredentials>(), It.IsAny<string>(), It.IsAny<string>(),It.IsAny<string>()))
                .ReturnsAsync(expectedResponse);

            mockAssumeRoleHelper.Setup(x => x.GetTemporaryCredentials()).ReturnsAsync(It.IsAny<Credentials>());
            //assert
            Assert.ThrowsAsync<ImageNotInsertedToS3>(() => classUnderTest.UploadImage(request));     
        }

        [Test]
        public void test_that_gateway_throws_document_not_inserted_when_s3_call_throws_an_exception()
        {
            //arrange
            var request = new ProcessImageData()
            {
                imageData = new Base64DecodedData(),
                imageId = faker.Random.Word(),
                processRef = faker.Random.Guid().ToString()
            };

            mockS3Client.Setup(x => x.insertImage(It.IsAny<AWSCredentials>(), It.IsAny<string>(), It.IsAny<string>(),It.IsAny<string>())).Throws<AggregateException>();
            mockAssumeRoleHelper.Setup(x => x.GetTemporaryCredentials()).ReturnsAsync(It.IsAny<Credentials>());
            //assert
            Assert.ThrowsAsync<ImageNotInsertedToS3>(() => classUnderTest.UploadImage(request));
        }

        #region Get Image
        [Test]
        public void test_that_gateway_returns_base64_string_for_successful_request()
        {
            //arrange
            var s3Response = new GetObjectResponse();
            var data = faker.Random.Guid().ToByteArray();
            s3Response.ResponseStream = new MemoryStream(data);
            s3Response.HttpStatusCode = HttpStatusCode.OK;
            mockS3Client.Setup(x => x.retrieveImage(It.IsAny<AWSCredentials>(), It.IsAny<string>(), It.IsAny<string>())).Returns(s3Response);
            //second response, as if reusing first one, the stream of data will be closed
            var s3Response2 = new GetObjectResponse();
            s3Response2.ResponseStream = new MemoryStream(data);
            var expectedResponse = ImageRetrievalFactory.EncodeStreamToBase64(s3Response2);
            //act
            var response = classUnderTest.RetrieveImage(It.IsAny<string>());
            Assert.NotNull(response);
            Assert.AreEqual(expectedResponse, response);         
        }

        [Test]
        public void test_that_gateway_throws_aggregate_exception_and_not_ImageNotFound_exception_if_s3_throws_exception_different_to_key_not_found()
        {
            //arrange
            var expectedException = new AggregateException();

            mockS3Client.Setup(x => x.retrieveImage(It.IsAny<AWSCredentials>(), It.IsAny<string>(), It.IsAny<string>())).Throws(expectedException);

            //assert
            Assert.Throws<AggregateException>(() => classUnderTest.RetrieveImage(It.IsAny<string>()));
        }
        [Test]
        public void test_that_gateway_throws_caught_exception_and_not_ImageNotFound_exception_if_s3_throws_exception_different_to_AggregateException()
        {
            //arrange
            var expectedException = new FieldAccessException();

            mockS3Client.Setup(x => x.retrieveImage(It.IsAny<AWSCredentials>(), It.IsAny<string>(), It.IsAny<string>())).Throws(expectedException);

            //assert
            Assert.Throws<FieldAccessException>(() => classUnderTest.RetrieveImage(It.IsAny<string>()));
        }

        [Test]
        public void test_that_gateway_throws_imageNotFound_exception_when_S3_throws_key_not_found_error()
        {
            //arrange
            var s3Exception = new AmazonS3Exception("The specified key does not exist.");
            var expectedException = new AggregateException("One or more errors occured.",s3Exception);
            mockS3Client.Setup(x => x.retrieveImage(It.IsAny<AWSCredentials>(), It.IsAny<string>(), It.IsAny<string>())).Throws(expectedException);
            //assert
            Assert.Throws<ImageNotFound>(() => classUnderTest.RetrieveImage(It.IsAny<string>()));
        }
        #endregion
    }
}
