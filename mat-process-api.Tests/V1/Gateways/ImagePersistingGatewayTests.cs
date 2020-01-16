using Amazon.Runtime;
using Amazon.S3.Model;
using Amazon.SecurityToken.Model;
using mat_process_api.V1.Exceptions;
using mat_process_api.V1.Gateways;
using mat_process_api.V1.Helpers;
using mat_process_api.V1.Infrastructure;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mat_process_api.Tests.V1.Gateways
{
    [TestFixture]
    public class ImagePersistingGatewayTests
    {
        private ImagePersistingGateway classUnderTest;
        private Mock<IAwsAssumeRoleHelper> mockAssumeRoleHelper;
        private Mock<IAmazonS3Client> mockS3Client;
        [SetUp]
        public void set_up()
        {
            mockAssumeRoleHelper = new Mock<IAwsAssumeRoleHelper>();
            mockS3Client = new Mock<IAmazonS3Client>();
            classUnderTest = new ImagePersistingGateway(mockS3Client.Object, mockAssumeRoleHelper.Object);
        }

        [Test]
        public void test_that_gateway_returns_does_not_throw_an_exception_when_successfully_inserting_an_image()
        {
            //arrange
            var request = "";
            var expectedResponse = new PutObjectResponse();
            expectedResponse.HttpStatusCode = System.Net.HttpStatusCode.NoContent;
            mockS3Client.Setup(x => x.insertImage(It.IsAny<AWSCredentials>(), It.IsAny<string>(), It.IsAny<string>())).Returns(expectedResponse);
            mockAssumeRoleHelper.Setup(x => x.GetTemporaryCredentials()).Returns(It.IsAny<Credentials>());
            //act
            Assert.DoesNotThrow(() => classUnderTest.UploadImage());
        }
    }
}
