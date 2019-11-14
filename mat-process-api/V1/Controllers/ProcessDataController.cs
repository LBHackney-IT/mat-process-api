using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mat_process_api.V1.Boundary;
using mat_process_api.V1.Domain;
using mat_process_api.V1.UseCase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace mat_process_api.V1.Controllers
{
    [ApiVersion("1")]
    [Route("api/v1/processData")]
    [ApiController]
    [Produces("application/json")]
    public class ProcessDataController : Controller
    {
        private IProcessData _processDataUsecase;
        private ILogger<ProcessDataController> _logger;

        public ProcessDataController(IProcessData processDataUsecase, ILogger<ProcessDataController> logger)
        {
            _processDataUsecase = processDataUsecase;
            _logger = logger;
        }

        /// <summary>
        /// Returns a process data object for a given process reference.
        /// Only one process data object will be returned at a time
        /// If no match is found then empty process data response object is returned with 200
        /// </summary>
        /// <param name="processRef"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(GetProcessDataResponse), 200)]
        [Route("{processRef}")]
        public IActionResult GetProcessData(string processRef)
        {
            _logger.LogInformation($"Get ProcessData request for process ID {processRef}");
            var request = new GetProcessDataRequest() {processRef = processRef};
            var result = _processDataUsecase.ExecuteGet(request);
            return Ok(result);
        }
    }
}
