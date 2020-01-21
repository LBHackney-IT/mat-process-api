using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace mat_process_api.V1.Boundary
{
    public class GetProcessImageRequest
    {
        [Required] public string processRef { get; set; }
        [Required] public string processType { get; set; }
        [Required] public string imageId { get; set; }
        [Required] public string fileExtension { get; set; }
    }
}
