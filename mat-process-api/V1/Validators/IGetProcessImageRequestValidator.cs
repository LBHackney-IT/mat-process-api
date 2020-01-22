using FluentValidation.Results;
using mat_process_api.V1.Boundary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mat_process_api.V1.Validators
{
    public interface IGetProcessImageRequestValidator
    {
        ValidationResult Validate(GetProcessImageRequest request);
    }
}
