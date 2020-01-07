using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mat_process_api.V1.Boundary;
using mat_process_api.V1.Factories;
using mat_process_api.V1.Domain;
using mat_process_api.V1.Gateways;

namespace mat_process_api.V1.UseCase
{
    public class ProcessDataUseCase :IProcessData
    {
        private IProcessDataGateway _processDataGateway;
        public ProcessDataUseCase(IProcessDataGateway processDataGateway)
        {
            _processDataGateway = processDataGateway;
        }
        public GetProcessDataResponse ExecuteGet(GetProcessDataRequest request)
        {
            var gatewayResult = _processDataGateway.GetProcessData(request.processRef);

            return new GetProcessDataResponse(request, gatewayResult,DateTime.Now);
        }
        public PostInitialProcessDocumentResponse ExecutePost(PostInitialProcessDocumentRequest request)
        {
            MatProcessData mappedRequest = ProcessDataFactory.CreateProcessDataObject(request); //mapping request into to be inserted domain object
            string gatewayResponse = _processDataGateway.PostInitialProcessDocument(mappedRequest);

            return new PostInitialProcessDocumentResponse(request, gatewayResponse, DateTime.Now);
        }
    }
}
