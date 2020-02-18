using mat_process_api.V1.Boundary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mat_process_api.V1.UseCase
{
    public interface IProcessImageUseCase
    {
        Task ExecutePost(PostProcessImageRequest request);
        GetProcessImageResponse ExecuteGet(GetProcessImageRequest request);
    }
}
