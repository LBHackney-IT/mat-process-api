using System;
using mat_process_api.Tests.V1.Helper;
using mat_process_api.V1.Boundary;
using mat_process_api.V1.Validators;
using NUnit.Framework;
using FluentValidation.TestHelper;

namespace mat_process_api.Tests.V1.Validators
{
    [TestFixture]
    public class PostProcessImageRequestValidatorTests
    {
        private PostProcessImageRequestValidator _postValidator;

        [SetUp]
        public void SetUp()
        {
            _postValidator = new PostProcessImageRequestValidator();
        }

        #region field is required tests

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void given_a_request_with_null_empty_or_whitespace_processRef_when_postProcessImageRequestValidator_is_called_then_it_returns_an_error(string processRef)
        {
            //arrange
            var request = MatProcessDataHelper.CreatePostProcessImageRequestObject();
            request.processRef = processRef;

            //act, assert
            _postValidator.ShouldHaveValidationErrorFor(req => req.processRef, request).WithErrorMessage("Process reference must be provided.");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void given_a_request_with_null_empty_or_whitespace_imageId_when_postProcessImageRequestValidator_is_called_then_it_returns_an_error(string imageId)
        {
            //arrange
            var request = MatProcessDataHelper.CreatePostProcessImageRequestObject();
            request.imageId = imageId;

            //act, assert
            _postValidator.ShouldHaveValidationErrorFor(req => req.imageId, request).WithErrorMessage("Image Id must be provided.");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void given_a_request_with_null_empty_or_whitespace_base64Image_string_when_postProcessImageRequestValidator_is_called_then_it_returns_an_error(string base64Image)
        {
            //arrange
            var request = MatProcessDataHelper.CreatePostProcessImageRequestObject();
            request.base64Image = base64Image;

            //act, assert
            _postValidator.ShouldHaveValidationErrorFor(req => req.base64Image, request).WithErrorMessage("Base64 Image string must be provided.");
        }

        #endregion

        #region format is invalid tests

        [TestCase("fe7ce9fb-8833-e877-a28c-f0857e6962b")] //less characters than it should contain
        [TestCase("6418aa75-f5ee6975-5279d2bcfee5f948")] //missing dashes
        [TestCase("a98e23-4abd0-83542-4b83225-72b6adc65")] //dashes in the wrong places
        [TestCase("ju5t50m3-w0rd-th4t-w1ll-m34nn0th1ng0")] //uses characters that are not among hexadecimal numerals
        public void given_a_request_with_invalid_processRef_guid_when_postProcessImageRequestValidator_is_called_then_it_returns_an_error(string processRef)
        {
            //arrange
            var request = MatProcessDataHelper.CreatePostProcessImageRequestObject();
            request.processRef = processRef;

            //act, assert
            _postValidator.ShouldHaveValidationErrorFor(req => req.processRef, request).WithErrorMessage("You need to provide a valid process reference.");
        }

        [TestCase("fe7ce7fb-7733-e877-a28c-f0857e6962b")] //less characters than it should contain
        [TestCase("6418aa75-f5ee6975-5279d7bcfee5f948")] //missing dashes
        [TestCase("a98e73-4abd0-83542-4b73225-72b6adc65")] //dashes in the wrong places
        [TestCase("ju5t70m3-w0rd-th4t-w1ll-m34nn0th1ng0")] //uses characters that are not among hexadecimal numerals
        public void given_a_request_with_invalid_ImageId_guid_when_postProcessImageRequestValidator_is_called_then_it_returns_an_error(string imageId)
        {
            //arrange
            var request = MatProcessDataHelper.CreatePostProcessImageRequestObject();
            request.imageId = imageId;

            //act, assert
            _postValidator.ShouldHaveValidationErrorFor(req => req.imageId, request).WithErrorMessage("You need to provide a valid Image Id.");
        }

        //[TestCase("data:image/jpeg;base64,/9j/4AAQSkZJRgABAQEASABI/2wBDAAgG\\BgcGBQgHwcJCQg")] //base64 bit contains characters that don't belong in base64 string
        //[TestCase("data:image/jpeg;base64,KVHFLFAApUcUaAG4pYo4pUADFKnYUOPUsUWA3FLFGjRYDcUqNKi")] //the base 64 bit of string is not of length multiple of 4
        //[TestCase("BAQFAwMEAQEJAQIDAAQREiExBUETIlFhBjJxgRSRobEjQlLt")] //doesn't start with 'data:image/{fileext};base64,'
        //[TestCase("data:imade/jpeg;base64,B0RXh8DNi8QcWJHJDgjRTkiVEc4Oisv/EABoBAAMBAQEBAAA")] //does start with the 'data:image/{fileext};base64,', but it has a typo
        //[TestCase("data:image/zz;base64,/1j/7TTQSkZJRgACAQEASBBI/2wBDAAgG\\BgcGBQgHwcJCQg")] //file extension in the data bit is less than 3 chars long 
        //public void given_a_request_with_an_invalid_base64Image_string_when_postProcessImageRequestValidator_is_called_then_it_returns_an_error(string base64Image)
        //{
        //    //arrange
        //    var request = MatProcessDataHelper.CreatePostProcessImageRequestObject();
        //    request.base64Image = base64Image;

        //    //act, assert
        //    _postValidator.ShouldHaveValidationErrorFor(req => req.base64Image, request).WithErrorMessage("You need to provide a valid Base64 Image string.");
        //}

        #endregion

        #region format is valid tests

        [Test]
        public void given_a_request_with_valid_processRef_guid_when_postProcessImageRequestValidator_is_called_then_it_returns_no_error()
        {
            //arange
            var request = MatProcessDataHelper.CreatePostProcessImageRequestObject();

            //act, assert
            _postValidator.ShouldNotHaveValidationErrorFor(req => req.processRef, request);
        }

        [Test]
        public void given_a_request_with_valid_ImageId_guid_when_postProcessImageRequestValidator_is_called_then_it_returns_no_error()
        {
            //arange
            var request = MatProcessDataHelper.CreatePostProcessImageRequestObject();

            //act, assert
            _postValidator.ShouldNotHaveValidationErrorFor(req => req.imageId, request);
        }

        [Test]
        public void given_a_request_with_valid_base64Image_string_when_postProcessImageRequestValidator_is_called_then_it_returns_no_error()
        {
            //arange
            var request = MatProcessDataHelper.CreatePostProcessImageRequestObject();

            //act, assert
            _postValidator.ShouldNotHaveValidationErrorFor(req => req.base64Image, request);
        }

        #endregion 
    }
}
