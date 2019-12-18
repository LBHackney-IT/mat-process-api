using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mat_process_api.V1.Boundary;
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

        public UpdateProcessDataResponse ExecuteUpdate(UpdateProcessDataRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
