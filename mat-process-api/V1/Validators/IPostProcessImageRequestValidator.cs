using System;
using FluentValidation.Results;
using mat_process_api.V1.Boundary;

namespace mat_process_api.V1.Validators
{
    public interface IPostProcessImageRequestValidator
    {
        ValidationResult Validate(PostProcessImageRequest request);
    }
}
