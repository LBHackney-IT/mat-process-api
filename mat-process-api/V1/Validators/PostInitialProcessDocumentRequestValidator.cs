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
            //RuleFor(postReq => postReq.processRef).NotNull().NotEmpty(); //process Reference needs to be provided
        }
    }
}
