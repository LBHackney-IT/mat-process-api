using Microsoft.Extensions.HealthChecks;
using mat_process_api.V1.Boundary;

namespace mat_process_api.UseCase.V1
{
    public class DbHealthCheckUseCase
    {
        private readonly IHealthCheckService _healthCheckService;

        public DbHealthCheckUseCase(IHealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        public HealthCheckResponse Execute()
        {
            var result = _healthCheckService.CheckHealthAsync().Result;

            bool success = result.CheckStatus == CheckStatus.Healthy;
            return new HealthCheckResponse(success, result.Description);
        }
    }

}
