using mat_process_api.V1.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mat_process_api.V1.Boundary
{
    public class UpdateProcessDataResponse
    {
        public UpdateProcessDataRequest Request { get; set; }
        public MatProcessData UpdatedProcessData { get; set; }
        public DateTime GeneratedAt { get; set; }

        public UpdateProcessDataResponse(UpdateProcessDataRequest request, MatProcessData processData, DateTime generatedAt)
        {
            Request = request;
            GeneratedAt = generatedAt;
            UpdatedProcessData = processData;
        }
    }
}
