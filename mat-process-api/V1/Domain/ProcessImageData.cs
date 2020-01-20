using System;
namespace mat_process_api.V1.Domain
{
    public class ProcessImageData
    {
        public string processRef { get; set; }
        public string imageId { get; set; }
        public string key { get; set; }

        public Base64DecodedData imageData { get; set; }
        
    }

    public class Base64DecodedData
    {
        public string imagebase64String { get; set; }
        public string imageType { get; set; }
        public string imageExtension { get; set; }
    }
}
