using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using mat_process_api.V1.Boundary;

namespace mat_process_api.V1.Validators
{
    public class PostInitialProcessDocumentRequestValidator : AbstractValidator<PostInitialProcessDocumentRequest>
    {
        public PostInitialProcessDocumentRequestValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(postReq => postReq.processRef).NotNull().WithMessage("Process reference must be provided.").NotEmpty().WithMessage("Process reference must be provided.").Must(ValidateGuid)
               .WithMessage("You need to provide a valid process reference.");
            RuleFor(postReq => postReq.processType).NotNull().WithMessage("Process type must be provided.")
                .DependentRules(() =>
                {
                    RuleFor(postReq => postReq.processType.value).NotEmpty().WithMessage("Process type value must be provided.")
                     .NotEmpty().WithMessage("Process type value must be provided."); ;
                    RuleFor(postReq => postReq.processType.name).NotNull().WithMessage("Process type name must be provided.")
                    .NotEmpty().WithMessage("Process type name must be provided.");
                });
            RuleFor(postReq => postReq.processDataSchemaVersion).NotEmpty().WithMessage("Process data schema version must be provided.");
        }
        private bool ValidateGuid(string guid)
        {
            return Guid.TryParse(guid, out var result);
        }
    }
}
