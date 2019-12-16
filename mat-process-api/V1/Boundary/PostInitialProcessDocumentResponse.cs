using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mat_process_api.V1.Domain;

namespace mat_process_api.V1.Boundary
{
    public class PostInitialProcessDocumentResponse
    {
        public GetProcessDataRequest Request { get; set; }
        public string ProcessRef { get; set; }
        public DateTime GeneratedAt { get; set; }

        public PostInitialProcessDocumentResponse(GetProcessDataRequest request, string processRef, DateTime generatedAt)
        {
            Request = request;
            GeneratedAt = generatedAt;
            ProcessRef = processRef;
        }
    }
}
