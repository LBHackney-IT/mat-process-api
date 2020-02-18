using Bogus;
using mat_process_api.V1.Domain;
using mat_process_api.V1.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mat_process_api.Tests.V1.Helper
{
    [TestFixture]
    public class ProcessImageDecoderTests
    {
        private ProcessImageDecoder _processImageDecoder;
        private Faker _faker = new Faker();

        [SetUp]
        public void SetUp()
        {
            _processImageDecoder = new ProcessImageDecoder();
        }

        [Test]
        public async Task given_a_base64ImageString_when_ProcessImageDecoder_is_called_then_it_returns_Base64DecodedData_with_correctly_decoded_byte_array() //in this case it's imageExtension
        {
            //arrange
            string base64ImageString = MatProcessDataHelper.CreatePostProcessImageRequestObject().base64Image;
            var decodedImageBytes = base64ImageString.Split(",")[1]; //expected decoded bytes

            //act
            var base64DecodedData = await _processImageDecoder.DecodeBase64ImageString(base64ImageString);

            //assert
            Assert.NotNull(base64DecodedData);
            Assert.AreEqual(decodedImageBytes,base64DecodedData.imagebase64String);
        }

        [Test]
        public async Task given_a_base64ImageString_when_ProcessImageDecoder_is_called_then_it_returns_Base64DecodedData_with_correctly_decoded_file_type() //in this case it's imageType
        {
            //arrange
            string base64ImageString = MatProcessDataHelper.CreatePostProcessImageRequestObject().base64Image;
            string decodedImageType = base64ImageString.Split(";")[0].Split(":")[1]; //expected File Type

            //act
            var base64DecodedData = await _processImageDecoder.DecodeBase64ImageString(base64ImageString);

            //assert
            Assert.IsInstanceOf<Base64DecodedData>(base64DecodedData);
            Assert.AreEqual(decodedImageType, base64DecodedData.imageType);
        }

        [Test]
        public async Task given_a_base64ImageString_when_ProcessImageDecoder_is_called_then_it_returns_Base64DecodedData_with_correctly_decoded_file_extension() //in this case it's imageExtension
        {
            //arrange
            string base64ImageString = MatProcessDataHelper.CreatePostProcessImageRequestObject().base64Image;
            string decodedImageExtension = base64ImageString.Split(";")[0].Split(":")[1].Split("/")[1]; //expected File Type

            //act
            var base64DecodedData = await _processImageDecoder.DecodeBase64ImageString(base64ImageString);

            //assert
            Assert.IsInstanceOf<Base64DecodedData>(base64DecodedData);
            Assert.AreEqual(decodedImageExtension, base64DecodedData.imageExtension);
        }
    }
}
