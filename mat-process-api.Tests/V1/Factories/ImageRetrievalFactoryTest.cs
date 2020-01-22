using Amazon.S3.Model;
using Bogus;
using mat_process_api.V1.Factories;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace mat_process_api.Tests.V1.Factories
{
    [TestFixture]
    public class ImageRetrievalFactoryTest
    {
        private Faker faker;
        [SetUp]
        public void set_up()
        {
            faker = new Faker();
        }
        public bool IsBase64String(string base64)
        {
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out int bytesParsed);
        }
        [Test]
        public void test_that_factory_can_decode_s3_response_to_base64()
        {
            //arrange
            var testResponse = new GetObjectResponse();
            testResponse.ResponseStream = new MemoryStream(faker.Random.Guid().ToByteArray());
            //act
            var result = ImageRetrievalFactory.EncodeStreamToBase64(testResponse);
            var resultBase64 = result.Split(",")[1];
            //assert
            Assert.IsNotEmpty(result);
            Assert.True(IsBase64String(resultBase64));
        }

        [Test]
        public void test_that_factory_returns_empty_string_if_s3_response_is_empty()
        {
            //arrange
            var testResponse = new GetObjectResponse(); //empty response
            //act
            var result = ImageRetrievalFactory.EncodeStreamToBase64(testResponse);
            //assert
            Assert.IsEmpty(result);
        }
    }
}
