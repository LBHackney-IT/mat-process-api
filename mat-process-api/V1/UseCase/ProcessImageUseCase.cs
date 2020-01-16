using System;
using mat_process_api.V1.Boundary;
using mat_process_api.V1.Domain;
using mat_process_api.V1.Factories;
using mat_process_api.V1.Gateways;
using mat_process_api.V1.Helpers;

namespace mat_process_api.V1.UseCase
{
    public class ProcessImageUseCase : IProcessImageUseCase
    {
        private IProcessImageGateway _processImageGateway;
        private IProcessImageDecoder _processImageDecoder;

        public ProcessImageUseCase(IProcessImageGateway gateway, IProcessImageDecoder imageDecoder)
        {
            _processImageGateway = gateway;
            _processImageDecoder = imageDecoder;
        }

        public void ExecutePost(PostProcessImageRequest request)
        {
            Base64DecodedData base64Decoded = _processImageDecoder.DecodeBase64ImageString(request.base64Image);
            ProcessImageData imageData = ImageDataFactory.CreateImageDataObject(request, base64Decoded);

            _processImageGateway.PostProcessImage(imageData);
        }
    }
}
