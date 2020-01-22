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
        [Test]
        public void given_an_null_processRef_the_validator_should_return_an_error()
        {
            //arrange
            string processRef = null;

            //assert
            validator.ShouldHaveValidationErrorFor(req => req.processRef, processRef);
        }

        [Test]
        public void given_an_empty_string_processRef_the_validator_should_return_an_error()
        {
            //arrange
            string processRef = "";

            //assert
            validator.ShouldHaveValidationErrorFor(req => req.processRef, processRef);
        }

        [Test]
        public void given_whitespace_processRef_the_validator_should_return_an_error()
        {
            //arrange
            string processRef = " ";

            //assert
            validator.ShouldHaveValidationErrorFor(req => req.processRef, processRef);
        }

        [Test]
        public void given_invalid_guid_string_the_validator_should_return_an_error()
        {
            //arrange
            string processRef = faker.Random.String();

            //assert
            validator.ShouldHaveValidationErrorFor(req => req.processRef, processRef);
        }

        [Test]
        public void given_valid_guid_string_the_validator_should_return_an_error()
        {
            //arrange
            string processRef = faker.Random.Guid().ToString();

            //assert
            validator.ShouldNotHaveValidationErrorFor(req => req.processRef, processRef);
        }
    }
    
}
