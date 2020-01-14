using System;
using mat_process_api.V1.Boundary;
using mat_process_api.V1.Domain;

namespace mat_process_api.V1.Factories
{
    public static class ImageDataFactory
    {
        public static ProcessImageData CreateImageDataObject(PostProcessImageRequest request, byte[] imageBytes)
        {
            return new ProcessImageData()
            {
                processRef = request.processRef,
                imageId = request.imageId,
                imageBytes = imageBytes
            };
        }
    }
}
