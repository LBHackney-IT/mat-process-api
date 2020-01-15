using System;
using System.Text.RegularExpressions;
using FluentValidation;
using mat_process_api.V1.Boundary;

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
            RuleFor(req => req.base64Image).NotNull().WithMessage("Base64 Image string must be provided.").NotEmpty().WithMessage("Base64 Image string must be provided.")
                .Must(ValidateBase64Length).WithMessage("You need to provide a valid Base64 Image string.")
                .Matches(new Regex(@"^data:image\/([^_\W]{3,});base64,([^_\W]|[+\/])+={0,3}$")).WithMessage("You need to provide a valid Base64 Image string.");
        }

        private bool ValidateGuid(string guid)
        {
            return Guid.TryParse(guid, out var result);
        }

        private bool ValidateBase64Length(string imageBase64)
        {
            var stringParts = imageBase64.Split(",");
            if (stringParts.Length != 2) { return false; }
            var base64partLenght = stringParts[1].Length;
            return 0 == base64partLenght % 4;
        }
    }
}
