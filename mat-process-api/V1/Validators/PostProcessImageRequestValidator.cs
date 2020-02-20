using FluentValidation;
using mat_process_api.V1.Boundary;
using System;

namespace mat_process_api.V1.Validators
{
    public class PostProcessImageRequestValidator : AbstractValidator<PostProcessImageRequest>, IPostProcessImageRequestValidator
    {
        public PostProcessImageRequestValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure; // Stops the validator from going into DependentRules or deeper into any single rule in general. This is needed because it doesn't make sense to keep checking if property is empty, once it was determined it's null.

            RuleFor(req => req.processRef).NotNull().WithMessage("Process reference must be provided.").NotEmpty().WithMessage("Process reference must be provided.").Must(ValidateGuid)
               .WithMessage("You need to provide a valid process reference.");
            RuleFor(req => req.imageId).NotNull().WithMessage("Image Id must be provided.").NotEmpty().WithMessage("Image Id must be provided.").Must(ValidateGuid)
                .WithMessage("You need to provide a valid Image Id.");
            RuleFor(req => req.base64Image).NotNull().WithMessage("Base64 Image string must be provided.").NotEmpty().WithMessage("Base64 Image string must be provided.");
        }

        private bool ValidateGuid(string guid)
        {
            return Guid.TryParse(guid, out var result);
        }
    }
}
