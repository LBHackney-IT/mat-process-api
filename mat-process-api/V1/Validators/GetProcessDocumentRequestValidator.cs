using FluentValidation;
using mat_process_api.V1.Domain;
using System;

namespace mat_process_api.V1.Validators
{
    public class GetProcessDocumentRequestValidator : AbstractValidator<GetProcessDataRequest>, IGetProcessDocumentRequestValidator
    {
        public GetProcessDocumentRequestValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(getRequest => getRequest.processRef)
                .NotNull().WithMessage("Process reference must be provided.")
                .NotEmpty().WithMessage("Process reference cannot be empty.")
                .Must(ValidateGuid).WithMessage("You need to provide a valid process reference.");
        }

        private bool ValidateGuid(string guid)
        {
            return Guid.TryParse(guid, out var result);
        }
    }
}
