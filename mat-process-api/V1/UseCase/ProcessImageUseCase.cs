using System;
using System.Threading.Tasks;
using mat_process_api.V1.Boundary;
using mat_process_api.V1.Domain;
using mat_process_api.V1.Factories;
using mat_process_api.V1.Gateways;
using mat_process_api.V1.Helpers;

namespace mat_process_api.V1.UseCase
{
    public class ProcessImageUseCase : IProcessImageUseCase
    {
        private IImagePersistingGateway _processImageGateway;
        private IProcessImageDecoder _processImageDecoder;

        public ProcessImageUseCase(IImagePersistingGateway gateway, IProcessImageDecoder imageDecoder)
        {
            _processImageGateway = gateway;
            _processImageDecoder = imageDecoder;
        }

        public async Task ExecutePost(PostProcessImageRequest request)
        {
            Base64DecodedData base64Decoded = await _processImageDecoder.DecodeBase64ImageString(request.base64Image);
            ProcessImageData imageData = await ImageDataFactory.CreateImageDataObject(request, base64Decoded);
            await _processImageGateway.UploadImage(imageData);
        }

        public GetProcessImageResponse ExecuteGet(GetProcessImageRequest request)
        {
            var imageKey = ImagePersistingHelper.generateImageKey(request.processType, request.imageId, request.processRef, request.fileExtension).Result; 
            var gatewayResponse = _processImageGateway.RetrieveImage(imageKey);

            return new GetProcessImageResponse(gatewayResponse, DateTime.Now, request);
        }
    }
}
