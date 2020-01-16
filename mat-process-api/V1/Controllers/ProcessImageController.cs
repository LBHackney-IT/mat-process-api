using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mat_process_api.V1.Boundary;
using mat_process_api.V1.UseCase;
using mat_process_api.V1.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        public ProcessImageController(IProcessImageUseCase usecase, IPostProcessImageRequestValidator postValidator)
        {
            _processImageUseCase = usecase;
            _postValidator = postValidator;
        }

        /// <summary>
        /// TODO: Add description
        /// </summary>
        /// <param name="imageData"></param>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult PostProcessImage([FromBody] PostProcessImageRequest imageData)
        {
            var validationResult = _postValidator.Validate(imageData);

            if (validationResult.IsValid)
            {
                _processImageUseCase.ExecutePost(imageData);

                return NoContent();
            }

            return BadRequest(validationResult.Errors);
        }

        [HttpGet]
        [Produces("application/json")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProcessImage([FromBody] GetProcessImageRequest imageData)
        {
            return Ok();
        }
    }
}
