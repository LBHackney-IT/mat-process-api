using FluentValidation.TestHelper;
using mat_process_api.Tests.V1.Helper;
using mat_process_api.V1.Validators;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mat_process_api.Tests.V1.Validators
{
    [TestFixture]
    public class GetProcessImageRequestValidatorTests
    {
        private GetProcessImageRequestValidator _getValidator;

        [SetUp]
        public void SetUp()
        {
            _getValidator = new GetProcessImageRequestValidator();
        }

        #region Format is invalid

        [TestCase("fe7ce9fb-8833-e877-a28c-f0857e6962b")] //less characters than it should contain
        [TestCase("6418aa75-f5ee6975-5279d2bcfee5f948")] //missing dashes
        [TestCase("a98e23-4abd0-83542-4b83225-72b6adc65")] //dashes in the wrong places
        [TestCase("ju5t50m3-w0rd-th4t-w1ll-m34nn0th1ng0")] //uses characters that are not among hexadecimal numerals
        public void given_a_request_with_invalid_processRef_guid_when_getProcessImageRequestValidator_is_called_then_it_returns_an_error(string processRef)
        {
            //arrange
            var request = MatProcessDataHelper.CreateGetProcessImageRequestObject();
            request.processRef = processRef;

            //act, assert
            _getValidator.ShouldHaveValidationErrorFor(req => req.processRef, request).WithErrorMessage("You need to provide a valid process reference.");
        }

        [TestCase("wg7ce7fb-5521-wc72-128c-g0937e6wf24")] //less characters than it should contain
        [TestCase("4128aa75-f5ee6075-52erd7bcfee5f948")] //missing dashes
        [TestCase("a23r73-4abd0-83782-4b72e25-71a6adc65")] //dashes in the wrong places
        [TestCase("ju5t70m3-w0rd-th4t-w1ll-m34nn0th1ng0")] //uses characters that are not among hexadecimal numerals
        public void given_a_request_with_invalid_ImageId_guid_when_getProcessImageRequestValidator_is_called_then_it_returns_an_error(string imageId)
        {
            //arrange
            var request = MatProcessDataHelper.CreateGetProcessImageRequestObject();
            request.imageId = imageId;

            //act, assert
            _getValidator.ShouldHaveValidationErrorFor(req => req.imageId, request).WithErrorMessage("You need to provide a valid Image Id.");
        }

        #endregion

        #region format is valid tests

        [Test]
        public void given_a_request_with_valid_processRef_guid_when_getProcessImageRequestValidator_is_called_then_it_returns_no_error()
        {
            //arange
            var request = MatProcessDataHelper.CreateGetProcessImageRequestObject();

            //act, assert
            _getValidator.ShouldNotHaveValidationErrorFor(req => req.processRef, request);
        }

        [Test]
        public void given_a_request_with_valid_ImageId_guid_when_getProcessImageRequestValidator_is_called_then_it_returns_no_error()
        {
            //arange
            var request = MatProcessDataHelper.CreateGetProcessImageRequestObject();

            //act, assert
            _getValidator.ShouldNotHaveValidationErrorFor(req => req.imageId, request);
        }

        #endregion
    }
}
