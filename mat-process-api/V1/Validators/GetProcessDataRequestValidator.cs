using System;
using FluentValidation;
using mat_process_api.V1.Domain;

namespace mat_process_api.V1.Validators
{
    public class GetProcessDataRequestValidator : AbstractValidator<GetProcessDataRequest>, IGetProcessDataRequestValidator
    {
        public GetProcessDataRequestValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure; // Stops the validator from going into DependentRules or deeper into any single rule in general. This is needed because it doesn't make sense to keep checking if property is empty, once it was determined it's null.
            RuleFor(postReq => postReq.processRef)
                .NotNull().WithMessage("Process reference must be provided.")
                .NotEmpty().WithMessage("Process reference cannot be empty.")
                .Must(ValidateGuid).WithMessage("You need to provide a valid process reference.")
                ;
                
        }

     private bool ValidateGuid(string guid)
        {
            return Guid.TryParse(guid, out var result);
        }    
    }

}
