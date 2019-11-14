using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mat_process_api.V1.Domain;

namespace mat_process_api.V1.Boundary
{
    public class GetProcessDataResponse
    {
        public GetProcessDataRequest Request { get; set; }
        public MatProcessData ProcessData { get; set; }
        public DateTime GeneratedAt { get; set; }

        public GetProcessDataResponse(GetProcessDataRequest request, MatProcessData processData, DateTime generatedAt)
        {
            Request = request;
            GeneratedAt = generatedAt;
            ProcessData = processData;
        }
    }
}
