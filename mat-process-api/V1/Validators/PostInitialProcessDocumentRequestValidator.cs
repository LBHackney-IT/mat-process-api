using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using mat_process_api.V1.Boundary;

namespace mat_process_api.V1.Validators
{
    public class PostInitialProcessDocumentRequestValidator : AbstractValidator<PostInitialProcessDocumentRequest>, IPostInitialProcessDocumentRequestValidator
    {
        public PostInitialProcessDocumentRequestValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure; // Stops the validator from going into DependentRules or deeper into any single rule in general. This is needed because for e.g. the 1st check of the Rule is whether the 'ProcessType' object is set, then the validation on the contents of that object follows within DependentRules. If this object is null, then it makes no sense to check it's properties, since they don't exist. In fact, you'd get a runtime error doing that.

            RuleFor(postReq => postReq.processRef).NotNull().WithMessage("Process reference must be provided.").NotEmpty().WithMessage("Process reference must be provided.").Must(ValidateGuid)
               .WithMessage("You need to provide a valid process reference.");
            RuleFor(postReq => postReq.processType).NotNull().WithMessage("Process type must be provided.")
                .DependentRules(() =>
                {
                    RuleFor(postReq => postReq.processType.value).NotNull().WithMessage("Process type value must be provided.")
                    .NotEmpty().WithMessage("Process type value must be provided.");
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
