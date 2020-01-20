using System;
using mat_process_api.V1.Boundary;
using mat_process_api.V1.Domain;
using mat_process_api.V1.Helpers;

namespace mat_process_api.V1.Factories
{
    public static class ImageDataFactory
    {
        public static ProcessImageData CreateImageDataObject(PostProcessImageRequest request, Base64DecodedData decodedStringData)
        {
            return new ProcessImageData()
            {
                processRef = request.processRef,
                imageId = request.imageId,
                imageData = decodedStringData,
                key = ImagePersistingHelper.generateImageKey(request.processType,request.imageId,request.processRef, decodedStringData.imageExtension)
            };
        }
    }
}
