using System;
namespace mat_process_api.V1.Domain
{
    public class ProcessImageData
    {
        public string processRef { get; set; }
        public string imageId { get; set; }
        public byte[] imageBytes { get; set; }
    }
}
