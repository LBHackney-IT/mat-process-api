using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mat_process_api.V1.Boundary;
using mat_process_api.V1.Domain;
using mat_process_api.V1.Gateways;
using mat_process_api.V1.UseCase;
using mat_process_api.V1.Validators;
using Microsoft.AspNetCore.Http;
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
        private IPostInitialProcessDocumentRequestValidator _postValidator;

        public ProcessDataController(IProcessData processDataUsecase, ILogger<ProcessDataController> logger, IPostInitialProcessDocumentRequestValidator postInitDocValidator)
        {
            _processDataUsecase = processDataUsecase;
            _logger = logger;
            _postValidator = postInitDocValidator;
        }

        /// <summary>
        /// Returns a process data object for a given process reference.
        /// Only one process data object will be returned at a time
        /// If no match is found then empty process data response object is returned with 200
        /// </summary>
        /// <param name="processRef"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{processRef}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GetProcessDataResponse), 200)]
        public IActionResult GetProcessData(string processRef)
        {
            _logger.LogInformation($"Get ProcessData request for process ID {processRef}");
            var request = new GetProcessDataRequest() {processRef = processRef };
            var result = _processDataUsecase.ExecuteGet(request);
            return Ok(result);
        }

        /// <summary>
        /// Updates proccess object JSON document by updating its "processData" property.
        /// If processData needs to be update, it's instead replaced with a full new one.
        /// Returning flat out 200 on successful update
        /// </summary>
        /// <param name="processDataJSON"></param>
        /// <returns></returns>
        [HttpPatch]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult UpdateExistingProcessDocument()
        {
            return Ok();
        }

        /// <summary>
        /// Creates an intial process JSON document in the database.
        /// Upon creating a resource returns 200 Ok
        /// </summary>
        /// <param name="processObject"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PostInitialProcessDocumentResponse), 201)]
        public IActionResult PostInitialProcessDocument([FromBody] PostInitialProcessDocumentRequest request)
        {
            //validate request
            var isRequestValid = _postValidator.Validate(request);
            if (isRequestValid.IsValid)
            {
                try
                {
                    PostInitialProcessDocumentResponse usecaseResponse = _processDataUsecase.ExecutePost(request);
                    return StatusCode(201, usecaseResponse);
                }
                catch(ConflictException ex)
                {
                    return Conflict("An error inserting an object with duplicate key has occured - " +
                        ex.InnerException);
                }
                catch(Exception ex)
                {
                    return StatusCode(500, "An error has occurred - " + ex.InnerException);
                }
            }
           
            return BadRequest(isRequestValid.Errors);         
        }
    }
}
