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
            //thrown if base64 string cannot be converted to valid Base64DecodedData object (validation cannot be done on boundary object at the moment for performance reasons)
            catch (ProcessImageDecoderException ex)
            {
                return BadRequest(ex.Message); 
            }
            //thrown if base64 string cannot be converted to byte array
            catch(Base64StringConversionToByteArrayException ex)
            {
                return BadRequest(ex.Message); 
            }
            //thrown if uploading image to S3 fails
            catch (ImageNotInsertedToS3 ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
            //thrown for any other exception
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        /// <summary>
        /// API Endpoint to retrieve images saved as part of MaT processes in S3
        /// </summary>
        [HttpGet]
        [Route("{processType}/{processRef}/{imageId}/{fileExtension}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GetProcessImageResponse), 200)]
        public IActionResult GetProcessImage([FromRoute] GetProcessImageRequest requestData)
        {
            _logger.LogInformation($"Get ProcessImage request for Process Type: {requestData.processType ?? "null"}, Process Reference: {requestData.processRef ?? "null"}, Image Id: {requestData.imageId ?? "null"} and File Extension: {requestData.fileExtension ?? "null"}");
            var validationResult = _getValidator.Validate(requestData);

            if (validationResult.IsValid)
            {
                try
                {
                    var usecaseResponse = _processImageUseCase.ExecuteGet(requestData);
                    return Ok(usecaseResponse);
                }
                catch(ImageNotFound ex)
                {
                    return NotFound($"The image with ID = {requestData.imageId} has not been found.");
                    throw ex;
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "An error has occured while processing the request - " + ex.Message + " " + ex.InnerException);
                }
            }

            _logger.LogInformation($"Get ProcessImage request for Process Type: {requestData.processType ?? "null"}, Process Reference: {requestData.processRef ?? "null"}, Image Id: {requestData.imageId ?? "null"} and File Extension: {requestData.fileExtension ?? "null"} did not pass the validation:\n\n{validationResult.Errors.Select(e => $"Validation error for: '{e.PropertyName}', message: '{e.ErrorMessage}'.").Aggregate((acc, m) => acc + "\n" + m)}");
            return BadRequest(validationResult.Errors);
        }
    }
}
