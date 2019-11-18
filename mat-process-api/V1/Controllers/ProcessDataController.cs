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
    [ApiVersion("1.0")]
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
        [Route("{propertyReference}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GetProcessDataResponse), 200)]
        public IActionResult GetProcessData(string propertyReference)
        {
            _logger.LogInformation($"Get ProcessData request for process ID {propertyReference}");
            var request = new GetProcessDataRequest() {processRef = propertyReference };
            var result = _processDataUsecase.ExecuteGet(request);
            return Ok(result);
        }

        /// <summary>
        /// Creates an intial JSON document in the database.
        /// Upon creating a resource returns 201
        /// If the supplied 'processObject' is invalid, then should return 400 --not implement yet
        /// </summary>
        /// <param name="processObject"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(object), 201)] // response object structure is tbd, so I put 'object' for the time being
        public IActionResult PostInitialProcessDocument()
        {
            return Created("api/v1/processData", new { _id = "someGUID_87g8iu8b", text = "shape_of_response_object_is_not_covered_in_documentation" });
        }
    }
}
