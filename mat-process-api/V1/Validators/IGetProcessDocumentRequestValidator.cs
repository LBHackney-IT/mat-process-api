using FluentValidation.Results;
using mat_process_api.V1.Domain;

namespace mat_process_api.V1.Validators
{
    public interface IGetProcessDocumentRequestValidator
    {
        ValidationResult Validate(GetProcessDataRequest processRef);
    }
}
