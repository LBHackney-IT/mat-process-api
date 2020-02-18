using Amazon.Runtime;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using mat_process_api.V1.Helpers;
using mat_process_api.V1.Infrastructure;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace mat_process_api.Tests.V1.Helper
{
    [TestFixture]
    public class AwsAssumeRoleHelperTests
    {
        private AwsAssumeRoleHelper awsAssumeRoleHelper;
        private Mock<IAmazonSTSClient> client;
        [SetUp]
        public void set_up()
        {
            client = new Mock<IAmazonSTSClient>();
            awsAssumeRoleHelper = new AwsAssumeRoleHelper(client.Object);
        }

        [TestCase]
        public void test_that_role_can_be_assumed_and_returns_temp_credentials()
        {
            //arrange
            var stsClient = new AmazonSecurityTokenServiceClient(It.IsAny<BasicAWSCredentials>());
            client.Setup(x => x.getStsClient(It.IsAny<BasicAWSCredentials>())).Returns(stsClient);
            var assumeRoleResponse = new AssumeRoleResponse() { Credentials = new Credentials() };
            client.Setup(x => x.assumeRole(It.IsAny<AmazonSecurityTokenServiceClient>(), It.IsAny<AssumeRoleRequest>()))
                .ReturnsAsync(assumeRoleResponse);
            //act
            var response = awsAssumeRoleHelper.GetTemporaryCredentials().Result;
            //assert
            Assert.IsInstanceOf<Credentials>(response);
            Assert.IsNotEmpty(response.AccessKeyId);
            Assert.IsNotEmpty(response.SecretAccessKey);
            Assert.IsNotEmpty(response.SessionToken);
        }
    }
}
