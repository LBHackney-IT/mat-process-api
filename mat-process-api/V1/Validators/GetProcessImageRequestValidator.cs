using FluentValidation;
using mat_process_api.V1.Boundary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mat_process_api.V1.Validators
{
    public class GetProcessImageRequestValidator : AbstractValidator<GetProcessImageRequest>, IGetProcessImageRequestValidator
    {
        public GetProcessImageRequestValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure; // Stops the validator from going into DependentRules or deeper into any single rule in general. This is needed because it doesn't make sense to keep checking if property is empty, once it was determined it's null.

            RuleFor(req => req.processRef).Must(ValidateGuid)
               .WithMessage("You need to provide a valid process reference.");
            RuleFor(req => req.imageId).Must(ValidateGuid)
                .WithMessage("You need to provide a valid Image Id.");
        }

        private bool ValidateGuid(string guid)
        {
            return Guid.TryParse(guid, out var result);
        }
    }
}
