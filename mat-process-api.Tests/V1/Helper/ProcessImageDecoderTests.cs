using Bogus;
using mat_process_api.V1.Domain;
using mat_process_api.V1.Exceptions;
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
        public void given_a_base64ImageString_when_ProcessImageDecoder_is_called_then_it_returns_Base64DecodedData_with_correctly_decoded_byte_array() //in this case it's imageExtension
        {
            //arrange
            string base64ImageString = MatProcessDataHelper.CreatePostProcessImageRequestObject("jpeg").base64Image;
            var decodedImageBytes = base64ImageString.Split(",")[1]; //expected decoded bytes

            //act
            var base64DecodedData = _processImageDecoder.DecodeBase64ImageString(base64ImageString);

            //assert
            Assert.NotNull(base64DecodedData);
            Assert.AreEqual(decodedImageBytes,base64DecodedData.imagebase64String);
        }

        [Test]
        public void given_a_base64ImageString_when_ProcessImageDecoder_is_called_then_it_returns_Base64DecodedData_with_correctly_decoded_file_type() //in this case it's imageType
        {
            //arrange
            string base64ImageString = MatProcessDataHelper.CreatePostProcessImageRequestObject("jpeg").base64Image;
            string decodedImageType = base64ImageString.Split(";")[0].Split(":")[1]; //expected File Type

            //act
            var base64DecodedData = _processImageDecoder.DecodeBase64ImageString(base64ImageString);

            //assert
            Assert.IsInstanceOf<Base64DecodedData>(base64DecodedData);
            Assert.AreEqual(decodedImageType, base64DecodedData.imageType);
        }

        [Test]
        public void given_a_base64ImageString_when_ProcessImageDecoder_is_called_then_it_returns_Base64DecodedData_with_correctly_decoded_file_extension() //in this case it's imageExtension
        {
            //arrange
            string base64ImageString = MatProcessDataHelper.CreatePostProcessImageRequestObject("jpeg").base64Image;
            string decodedImageExtension = base64ImageString.Split(";")[0].Split(":")[1].Split("/")[1]; //expected File Type

            //act
            var base64DecodedData = _processImageDecoder.DecodeBase64ImageString(base64ImageString);

            //assert
            Assert.IsInstanceOf<Base64DecodedData>(base64DecodedData);
            Assert.AreEqual(decodedImageExtension, base64DecodedData.imageExtension);
        }

        //boundary object validation for base64 string property has been removed to improve performance
        //gateway will handle some validation errors using exceptions, but these are required to cover the ones not caught by the gateway
        [TestCase("data:imade/jpeg;base64,B0RXh8DNi8QcWJHJDgjRTkiVEc4Oisv/EABoBAAMBAQEBAAA")] //does start with the 'data:image/{fileext};base64,', but it has a typo
        [TestCase("data:image/zz;base64,B0RXh8DNi8QcWJHJDgjRTkiVEc4Oisv/EABoBAAMBAQEBAAA")] //file extension in the data bit is not valid
        [TestCase("data:image/jpeg;base64B0RXh8DNi8QcWJHJDgjRTkiVEc4Oisv/EABoBAAMBAQEBAAA")] //missing comma after base64
        public void given_invalid_filetype_is_passed_in_base64ImageString_when_ProcessImageDecoder_is_called_then_decoder_throws_ProcessImageDecoderException(string base64String)
        {
            //arrange
            var expectedException = new ProcessImageDecoderException();
            
            //assert
            Assert.Throws<ProcessImageDecoderException>(() => _processImageDecoder.DecodeBase64ImageString(base64String));
        }

        [TestCase("data:image/jpeg;base64,B0RXh8DNi8QcWJHJDgjRTkiVEc4Oisv/EABoBAAMBAQEBAAA")] //jpeg
        [TestCase("data:image/png;base64,B0RXh8DNi8QcWJHJDgjRTkiVEc4Oisv/EABoBAAMBAQEBAAA")] //png
        [TestCase("data:image/bmp;base64,B0RXh8DNi8QcWJHJDgjRTkiVEc4Oisv/EABoBAAMBAQEBAAA")] //bmp
        [TestCase("data:image/gif;base64,B0RXh8DNi8QcWJHJDgjRTkiVEc4Oisv/EABoBAAMBAQEBAAA")] //gif
        public void given_valid_filetype_is_passed_in_base64ImageString_when_ProcessImageDecoder_is_called_then_decoder_does_not_throw_an_error(string base64String)
        {
            //act
            var result = _processImageDecoder.DecodeBase64ImageString(base64String);

            //assert
            Assert.IsInstanceOf<Base64DecodedData>(result);
        }
    }
}
