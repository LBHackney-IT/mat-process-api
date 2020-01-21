using System;
using FluentValidation;
using FluentValidation.Results;
using mat_process_api.V1.Domain;

namespace mat_process_api.V1.Validators
{
    public interface IGetProcessDataRequestValidator
    {
        ValidationResult Validate(GetProcessDataRequest processRef);
    }
}
