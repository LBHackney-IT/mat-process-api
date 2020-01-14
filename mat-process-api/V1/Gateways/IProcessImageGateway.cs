using System;
using mat_process_api.V1.Boundary;
using mat_process_api.V1.Domain;

namespace mat_process_api.V1.Gateways
{
    public interface IProcessImageGateway
    {
        void PostProcessImage(ProcessImageData request);
    }
}
