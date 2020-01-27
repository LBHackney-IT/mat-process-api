using Bogus;
using FluentValidation.TestHelper;
using mat_process_api.V1.Validators;
using NUnit.Framework;

namespace mat_process_api.Tests.V1.Validators
{

    [TestFixture]
    public class GetProcessRequestValidatorTest
    {
        GetProcessDocumentRequestValidator validator;
        private readonly Faker faker = new Faker();

        [SetUp]

        public void SetUp()
        {
            validator = new GetProcessDocumentRequestValidator();
        }

        [TestCase(null)]
        public void given_null_processRef_the_GetProcessDocumentRequestValidator_should_return_a_correct_error_message(string processRef)
        {
            //assert
            var result = validator.ShouldHaveValidationErrorFor(req => req.processRef, processRef);
            result.WithErrorMessage("Process reference must be provided.");
        }

        [TestCase("")]
        [TestCase(" ")]
        public void given_empty_or_whitespace_processRef_the_GetProcessDocumentRequestValidator_should_return_a_correct_error_message(string processRef)
        {
            //assert
            var result = validator.ShouldHaveValidationErrorFor(req => req.processRef, processRef);
            result.WithErrorMessage("Process reference cannot be empty.");
        }

        [TestCase("fe7ce7fb-7733-e877-a28c-f0857e6962b")] //less characters than it should contain
        [TestCase("6418aa75-f5ee6975-5279d7bcfee5f948")] //missing dashes
        [TestCase("a98e73-4abd0-83542-4b73225-72b6adc65")] //dashes in the wrong places
        [TestCase("ju5t70m3-w0rd-th4t-w1ll-m34nn0th1ng0")] //uses characters that are not among hexadecimal numerals
        public void given_invalid_guid_string_the_validator_should_return_an_error(string processRef)
        {
            //assert
            validator.ShouldHaveValidationErrorFor(req => req.processRef, processRef);
        }

        [Test]
        public void given_valid_guid_string_for_processRef_the_GetProcessDocumentRequestValidator_should_not_return_an_error()
        {
            //arrange
            string processRef = faker.Random.Guid().ToString();

            //assert
            validator.ShouldNotHaveValidationErrorFor(req => req.processRef, processRef);
        }
    }
    
}
