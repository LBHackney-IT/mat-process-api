using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mat_process_api.V1.Boundary
{
    public class PostProcessImageRequest
    {
        public string processRef { get; set; }
        public string imageId { get; set; }
        public string base64Image { get; set; }
    }
}
