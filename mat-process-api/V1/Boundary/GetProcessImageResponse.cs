using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mat_process_api.V1.Boundary
{
    public class GetProcessImageResponse
    {
        public string Base64Image { get; set; }
        public DateTime GeneratedAt { get; set; }
        public GetProcessImageRequest Request { get; set; }

        public GetProcessImageResponse(string base64image, DateTime generatedAt, GetProcessImageRequest request)
        {
            Base64Image = base64image;
            GeneratedAt = generatedAt;
            Request = request;
        } 
    }
}
