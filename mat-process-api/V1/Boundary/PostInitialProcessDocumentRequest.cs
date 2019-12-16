using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace mat_process_api.V1.Boundary
{
    public class PostInitialProcessDocumentRequest
    {
        [Required] public string processRef { get; set; }
        [Required] public ProcessType processType { get; set; }
        [Required] public int processDataSchemaVersion { get; set; }
    }

    public class ProcessType
    {
        [Required] public int value { get; set; }
        [Required] public string name { get; set; }
    }
}
