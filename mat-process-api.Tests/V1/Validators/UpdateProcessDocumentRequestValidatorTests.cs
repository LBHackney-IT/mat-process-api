using Bogus;
using FluentValidation.TestHelper;
using mat_process_api.V1.Boundary;
using mat_process_api.V1.Domain;
using mat_process_api.V1.Validators;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mat_process_api.Tests.V1.Validators
{
    [TestFixture]
    public class UpdateProcessDocumentRequestValidatorTests
    {
    private UpdateProcessDocumentRequestValidator _updateProcessDocValidator;
    private Faker _faker = new Faker();
    string randomGuid;

        [SetUp]
        public void SetUp()
        {
            _updateProcessDocValidator = new UpdateProcessDocumentRequestValidator();
            randomGuid = _faker.Random.Guid().ToString();
        }

        [TestCase(" ")]
        [TestCase("")]
        [TestCase(null)]
        public void given_an_empty_process_ref_validator_returns_correct_error_message(string processReference)
        {
            var updateData = new MatProcessData();
            updateData.Id = processReference;
            var request = new UpdateProcessDataRequest() { processDataToUpdate = updateData };
            _updateProcessDocValidator.ShouldHaveValidationErrorFor(r => r.processDataToUpdate.Id, request).WithErrorMessage("Process reference must be provided.");
        }
        [TestCase("abv")]
        [TestCase("123-bca-45-65657-aaa")]
        public void given_an_invalid_process_ref_validator_returns_correct_error_message(string processReference)
        {
            var updateData = new MatProcessData();
            updateData.Id = processReference;
            var request = new UpdateProcessDataRequest() { processDataToUpdate = updateData };
            _updateProcessDocValidator.ShouldHaveValidationErrorFor(r => r.processDataToUpdate.Id, request).WithErrorMessage("You need to provide a valid process reference.");
        }

        [Test]
        public void given_missing_fields_to_update_in_request_object_validator_returns_correct_error_message()
        {
            var updateData = new MatProcessData();
            updateData.Id = randomGuid;
            var request = new UpdateProcessDataRequest() { processDataToUpdate = updateData };
            _updateProcessDocValidator.ShouldHaveValidationErrorFor(r => r.processDataToUpdate,request.processDataToUpdate).WithErrorMessage("At least one object must be providef for an update.");
        }

        [Test]
        public void given_a_valid_request_validator_should_return_no_errors()
        {
            var updateData = new MatProcessData();
            updateData.Id = randomGuid;
            updateData.DateCompleted = _faker.Date.Recent();
            var request = new UpdateProcessDataRequest() { processDataToUpdate = updateData };
            _updateProcessDocValidator.ShouldNotHaveValidationErrorFor(r => r.processDataToUpdate, request.processDataToUpdate);
        }

    }
}
