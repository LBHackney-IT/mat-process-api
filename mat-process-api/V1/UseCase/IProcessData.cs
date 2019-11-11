using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mat_process_api.V1.Boundary;
using mat_process_api.V1.Domain;

namespace mat_process_api.V1.UseCase
{
    public interface IProcessData
    {
        GetProcessDataResponse ExecuteGet(GetProcessDataRequest request);
    }
}
