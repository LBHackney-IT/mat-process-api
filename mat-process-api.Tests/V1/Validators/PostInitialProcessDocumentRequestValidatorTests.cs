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

        [SetUp]
        public void SetUp()
        {
            _postInitProcessDocValidator = new PostInitialProcessDocumentRequestValidator();
        }

        #region Field is Required Tests

        [TestCase(" ")]
        [TestCase("")]
        [TestCase(null)]
        public void given_a_postInitialProcessRequest_object_with_processRef_that_is_set_to_a_whitespace_or_null_or_empty_value_when_validator_is_called_it_returns_an_error(string processReference)
        {
            var request = new PostInitialProcessDocumentRequest() { processRef = processReference };
            _postInitProcessDocValidator.ShouldHaveValidationErrorFor(r => r.processRef, request).WithErrorMessage("Process reference needs to be provided");
        }

        [TestCase(0)] //not zero, because it's a default integer value that could be set if nothing is provided
        [TestCase(null)]
        public void given_a_postInitialProcessRequest_object_with_processDataSchemaVersion_that_is_set_to_zero_or_null_value_when_validator_is_called_it_returns_an_error(int processDataSchemaVersion)
        {
            var request = new PostInitialProcessDocumentRequest() { processDataSchemaVersion = processDataSchemaVersion };
            _postInitProcessDocValidator.ShouldHaveValidationErrorFor(r => r.processDataSchemaVersion, request).WithErrorMessage("Process data schema version needs to be provided. It has to be 1 or higher.");
        }

        [Test]
        public void given_a_postInitialProcessRequest_object_with_processType_object_that_is_set_to_null_value_when_validator_is_called_it_returns_an_error()
        {
            var request = new PostInitialProcessDocumentRequest() { processType = null };
            _postInitProcessDocValidator.ShouldHaveValidationErrorFor(r => r.processType, request).WithErrorMessage("Process type object needs to be provided. It has to contain the 'value' and 'name' fields.");
        }

        [TestCase(0)]
        [TestCase(null)]
        public void given_a_postInitialProcessRequest_object_with_processType_object_whos_value_field_is_set_to_zero_or_null_value_when_validator_is_called_it_returns_an_error(int processTypeValue)
        {
            var request = new PostInitialProcessDocumentRequest() { processType = new ProcessType() { value = processTypeValue } };
            _postInitProcessDocValidator.ShouldHaveValidationErrorFor(r => r.processType.value, request).WithErrorMessage("Process type value field needs to be provided.");
        }

        [TestCase(" ")]
        [TestCase("")]
        [TestCase(null)]
        public void given_a_postInitialProcessRequest_object_with_processType_object_whos_name_field_is_set_to_a_whitespace_or_null_or_empty_value_when_validator_is_called_it_returns_an_error(string processTypeName)
        {
            var request = new PostInitialProcessDocumentRequest() { processType = new ProcessType() { name = processTypeName } };
            _postInitProcessDocValidator.ShouldHaveValidationErrorFor(r => r.processType.name, request).WithErrorMessage("Process type name field needs to be provided");
        }

        #endregion

        #region Guid is Valid Test

        [Test]
        public void given_a_postInitialProcessRequest_object_with_a_valid_processRef_when_validator_is_called_it_does_NOT_return_an_error()
        {
            string randomGuid = _faker.Random.Guid().ToString();
            var request = new PostInitialProcessDocumentRequest() { processRef = randomGuid };
            _postInitProcessDocValidator.ShouldNotHaveValidationErrorFor(r => r.processRef, request);
        }

        [TestCase("fe7cd9fb-8833-e877-a28c-f0557e6962b")] //less characters than it should contain
        [TestCase("6418aa75-f5ee6975-5279d2bcfee5f948")] //missing dashes
        [TestCase("a68e23-4abd0-83542-4b89425-72b6adc65")] //dashes in the wrong places
        [TestCase("ju5t50m3-w0rd-th4t-w1ll-m34nn0th1ng7")] //uses characters that are not among hexadecimal numerals
        [TestCase("a68e234abd0835424b8942572b6adc65")] //no dashes at all, but this might just not trigger the error - depends on Microsofts implementation of TryParse
        public void given_a_postInitialProcessRequest_object_with_an_invalid_processRef_when_validator_is_called_it_returns_an_error(string processReference)
        {
            var request = new PostInitialProcessDocumentRequest() { processRef = processReference };
            _postInitProcessDocValidator.ShouldHaveValidationErrorFor(r => r.processRef, request);
        }

        //official website - basic setup of validator with an example of how to use it  https://fluentvalidation.net/start#creating-your-first-validator
        //the validator class is already setup in main project Validators folder - it doesn't implement any interface yet as it didn't need one.
        //this is how we did tests on Addresses API: https://github.com/LBHackney-IT/HackneyAddressesAPI/blob/master/LBHAddressesAPITest/Validation/SearchAddressValidatorTests.cs
        //this is our implementation of fluent validor class: https://github.com/LBHackney-IT/HackneyAddressesAPI/blob/master/HackneyAddressesAPI/Validation/SearchAddressValidator.cs
        //you might need to do something bit custom for the GUID validity check - it's been poorly documented on the official site when we did it, so
        //the example from our implementation would be the Lines: 28,29 and 31. Where it uses Must to call a custom validation method defined bellow.

        #endregion
    }
}
