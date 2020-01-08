using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Results;
using mat_process_api.V1.Boundary;

namespace mat_process_api.V1.Validators
{
    public interface IPostInitialProcessDocumentRequestValidator
    {
        ValidationResult Validate(PostInitialProcessDocumentRequest instance);
    }
}
