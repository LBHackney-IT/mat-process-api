using mat_process_api.V1.Validators;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.TestHelper;
using mat_process_api.V1.Boundary;
using Bogus;

namespace mat_process_api.Tests.V1.Validators
{
    [TestFixture]
    public class PostInitialProcessDocumentRequestValidatorTests
    {
        private PostInitialProcessDocumentRequestValidator _postInitProcessDocValidator;
        private Faker _faker = new Faker();
        string randomGuid;

        [SetUp]
        public void SetUp()
        {
            _postInitProcessDocValidator = new PostInitialProcessDocumentRequestValidator();
            randomGuid = _faker.Random.Guid().ToString();
        }

        #region Field is Required Tests

        [TestCase(" ")]
        [TestCase("")]
        [TestCase(null)]
        public void given_a_postInitialProcessRequest_object_with_processRef_that_is_set_to_a_whitespace_or_null_or_empty_value_when_validator_is_called_it_returns_an_error(string processReference)
        {
            var request = new PostInitialProcessDocumentRequest() { processRef = processReference };
            _postInitProcessDocValidator.ShouldHaveValidationErrorFor(r => r.processRef, request).WithErrorMessage("Process reference must be provided.");
        }

        [TestCase(0)] //not zero, because it's a default integer value that could be set if nothing is provided
        [TestCase(null)]
        public void given_a_postInitialProcessRequest_object_with_processDataSchemaVersion_that_is_set_to_zero_or_null_value_when_validator_is_called_it_returns_an_error(int processDataSchemaVersion)
        {
            var request = new PostInitialProcessDocumentRequest() { processDataSchemaVersion = processDataSchemaVersion };
            _postInitProcessDocValidator.ShouldHaveValidationErrorFor(r => r.processDataSchemaVersion, request).WithErrorMessage("Process data schema version must be provided.");
        }

        [Test]
        public void given_a_postInitialProcessRequest_object_with_processType_object_that_is_set_to_null_value_when_validator_is_called_it_returns_an_error()
        {
            var request = new PostInitialProcessDocumentRequest() { processType = null };
            _postInitProcessDocValidator.ShouldHaveValidationErrorFor(r => r.processType, request).WithErrorMessage("Process type must be provided.");
        }

        [TestCase(0)]
        [TestCase(null)]
        public void given_a_postInitialProcessRequest_object_with_processType_object_whos_value_field_is_set_to_zero_or_null_value_when_validator_is_called_it_returns_an_error(int processTypeValue)
        {
            var request = new PostInitialProcessDocumentRequest() { processType = new ProcessType() { value = processTypeValue } };
            _postInitProcessDocValidator.ShouldHaveValidationErrorFor(r => r.processType.value, request).WithErrorMessage("Process type value must be provided.");
        }

        [TestCase(" ")]
        [TestCase("")]
        [TestCase(null)]
        public void given_a_postInitialProcessRequest_object_with_processType_object_whos_name_field_is_set_to_a_whitespace_or_null_or_empty_value_when_validator_is_called_it_returns_an_error(string processTypeName)
        {
            var request = new PostInitialProcessDocumentRequest() { processType = new ProcessType() { name = processTypeName } };
            _postInitProcessDocValidator.ShouldHaveValidationErrorFor(r => r.processType.name, request).WithErrorMessage("Process type name must be provided.");
        }

        #endregion

        #region Guid is Valid Test

        [Test]
        public void given_a_postInitialProcessRequest_object_with_a_valid_processRef_when_validator_is_called_it_does_NOT_return_an_error()
        {
            
            var request = new PostInitialProcessDocumentRequest() { processRef = randomGuid };
            _postInitProcessDocValidator.ShouldNotHaveValidationErrorFor(r => r.processRef, request);
        }

        [TestCase("fe7cd9fb-8833-e877-a28c-f0557e6962b")] //less characters than it should contain
        [TestCase("6418aa75-f5ee6975-5279d2bcfee5f948")] //missing dashes
        [TestCase("a68e23-4abd0-83542-4b89425-72b6adc65")] //dashes in the wrong places
        [TestCase("ju5t50m3-w0rd-th4t-w1ll-m34nn0th1ng7")] //uses characters that are not among hexadecimal numerals
        public void given_a_postInitialProcessRequest_object_with_an_invalid_processRef_when_validator_is_called_it_returns_an_error(string processReference)
        {
            var request = new PostInitialProcessDocumentRequest() { processRef = processReference };
            _postInitProcessDocValidator.ShouldHaveValidationErrorFor(r => r.processRef, request);
        }

        #endregion
    }
}
