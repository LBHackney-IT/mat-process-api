using System;
using FluentValidation.TestHelper;
using mat_process_api.V1.Validators;
using Moq;
using NUnit.Framework;

namespace mat_process_api.Tests.V1.Validators
{
    [TestFixture]
    public class GetProcessRequestValidatorTest
    {
        GetProcessDataRequestValidator validator;
        [SetUp]

        public void SetUp()
        {
            validator = new GetProcessDataRequestValidator();
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
            string processRef = "bob";

            //assert
            validator.ShouldHaveValidationErrorFor(req => req.processRef, processRef);
        }

        [Test]
        public void given_valid_guid_string_the_validator_should_return_an_error()
        {
            //arrange
            string processRef = "2539ca27-12c0-e811-a96c-002248072cc3";

            //assert
            validator.ShouldNotHaveValidationErrorFor(req => req.processRef, processRef);
        }


    }
}

