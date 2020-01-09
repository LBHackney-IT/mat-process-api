using mat_process_api.V1.Boundary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using mat_process_api.V1.Domain;

namespace mat_process_api.V1.Validators
{
    public class UpdateProcessDocumentRequestValidator : AbstractValidator<UpdateProcessDataRequest>, IUpdateProcessDocumentRequestValidator
    {
        public UpdateProcessDocumentRequestValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure; // Stops the validator from going into DependentRules or deeper into any single rule in general. This is needed because for e.g. the 1st check of the Rule is whether the 'ProcessType' object is set, then the validation on the contents of that object follows within DependentRules. If this object is null, then it makes no sense to check it's properties, since they don't exist. In fact, you'd get a runtime error doing that.

            RuleFor(updateRequest => updateRequest.processDataToUpdate.Id).NotNull().WithMessage("Process reference must be provided.").NotEmpty()
               .WithMessage("Process reference must be provided.").Must(ValidateGuid)
               .WithMessage("You need to provide a valid process reference.");
            RuleFor(updateRequest => updateRequest.processDataToUpdate).NotNull().WithMessage("Request object is missing.")
                .DependentRules(() =>
                {
                    RuleFor(updateRequest => updateRequest.processDataToUpdate).NotNull().Must(ValidateAnUpdateIsProvided)
                        .WithMessage("At least one object must be providef for an update.");
                });
        }

        private bool ValidateGuid(string guid)
        {
            return Guid.TryParse(guid, out var result);
        }

        private bool ValidateAnUpdateIsProvided(MatProcessData request)
        { //user must provide at least one object to update
            if (request.PostProcessData != null)
            {
                return true;
            }
            else if (request.PreProcessData != null)
            {
                return true;
            }
            else if (request.ProcessDataSchemaVersion != 0)
            {
                return true;
            }
            else if (request.ProcessData != null)
            {
                return true;
            }
            else if (request.ProcessStage != null)
            {
                return true;
            }
            else if (request.DateCompleted != null && request.DateCompleted != DateTime.MinValue)
            {
                return true;
            }
            return false; //if non of the objects were non empty
        }
    }
}
