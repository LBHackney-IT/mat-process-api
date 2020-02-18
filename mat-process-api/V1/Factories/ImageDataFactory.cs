using System;
using System.Threading.Tasks;
using mat_process_api.V1.Boundary;
using mat_process_api.V1.Domain;
using mat_process_api.V1.Helpers;

namespace mat_process_api.V1.Factories
{
    public class ImageDataFactory
    {
        public static async Task<ProcessImageData> CreateImageDataObject(PostProcessImageRequest request, Base64DecodedData decodedStringData)
        {
            string key = await ImagePersistingHelper.generateImageKey(request.processType, request.imageId, request.processRef, decodedStringData.imageExtension);

            return new ProcessImageData()
            {
                processRef = request.processRef,
                imageId = request.imageId,
                imageData = decodedStringData,
                key = key
            };
        }
    }
}
