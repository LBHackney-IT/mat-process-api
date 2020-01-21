using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mat_process_api.V1.Boundary;
using mat_process_api.V1.Exceptions;
using mat_process_api.V1.UseCase;
using mat_process_api.V1.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace mat_process_api.V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v1/processImageData")] //<----- do we change this? url undecided
    [ApiController]
    [Produces("application/json")]
    public class ProcessImageController : Controller
    {
        private IProcessImageUseCase _processImageUseCase;
        private IPostProcessImageRequestValidator _postValidator;
        private IGetProcessImageRequestValidator _getValidator;
        private ILogger<ProcessImageController> _logger;

        public ProcessImageController(IProcessImageUseCase usecase, IPostProcessImageRequestValidator postValidator, IGetProcessImageRequestValidator getValidator, ILogger<ProcessImageController> logger)
        {
            _processImageUseCase = usecase;
            _postValidator = postValidator;
            _getValidator = getValidator;
            _logger = logger;
        }

        /// <summary>
        /// API endpoint to insert images in base64 format into S3
        /// </summary>
        /// <param name="imageData"></param>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult PostProcessImage([FromBody] PostProcessImageRequest imageData)
        {
            try
            {
                var validationResult = _postValidator.Validate(imageData);

                if (validationResult.IsValid)
                {
                    _processImageUseCase.ExecutePost(imageData);

                    return NoContent();
                }

                return BadRequest(validationResult.Errors);
            }
            catch(ImageNotInsertedToS3 ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        /// <summary>
        /// API Endpoint to retrieve images saved as part of MaT processes in S3
        /// </summary>
        /// <param name="processType">The process type (name) - e.g. Tenancy and household check </param>
        /// <param name="processRef">The process reference, that the image is saved against</param>
        /// <param name="imageId">The ID of the image to be retrieved</param>
        /// <param name="fileExtension">The file extension of the file to be retrieved</param>
        [HttpGet]
        [Route("{processType}/{processRef}/{imageId}/{fileExtension}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GetProcessImageResponse), 200)]
        public IActionResult GetProcessImage(string processType, string processRef, string imageId,string fileExtension)
        {
            _logger.LogInformation($"Get ProcessImage request for process reference: {processRef} and image Id: {imageId}");
            var requestData = new GetProcessImageRequest { processType = processType, processRef = processRef, imageId = imageId, fileExtension = fileExtension };
            var validationResult = _getValidator.Validate(requestData);

            if (validationResult.IsValid)
            {
                try
                {
                    var usecaseResponse = _processImageUseCase.ExecuteGet(requestData);
                    return Ok(usecaseResponse);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "An error has occured while processing the request - " + ex.Message + " " + ex.InnerException);
                }
            }

            _logger.LogInformation($"The Get ProcessImage request with process reference: {requestData.processRef ?? "null"} and image Id: {requestData.imageId ?? "null"} did not pass the validation:\n\n{validationResult.Errors.Select(e => $"Validation error for: '{e.PropertyName}', message: '{e.ErrorMessage}'.").Aggregate((acc, m) => acc + "\n" + m)}");
            return BadRequest(validationResult.Errors);
        }
    }
}
