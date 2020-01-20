using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace mat_process_api.V1.Boundary
{
    public class PostInitialProcessDocumentRequest
    {
        public string processRef { get; set; }
        public ProcessType processType { get; set; }
        public int processDataSchemaVersion { get; set; }
    }

    public class ProcessType
    {
        public int value { get; set; }
        public string name { get; set; }
    }
}
