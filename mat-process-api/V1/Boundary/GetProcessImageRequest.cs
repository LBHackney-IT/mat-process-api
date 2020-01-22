using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace mat_process_api.V1.Boundary
{
    public class GetProcessImageRequest
    {
        [FromRoute]
        public string processRef { get; set; }
        public string processType { get; set; }
        public string imageId { get; set; }
        public string fileExtension { get; set; }
    }
}
