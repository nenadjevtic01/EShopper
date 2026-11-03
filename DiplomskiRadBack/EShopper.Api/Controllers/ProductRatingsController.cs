using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.Implementation;

namespace EShopper.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductRatingsController : ControllerBase
    {
        private readonly UseCaseHandler _handler;

        public ProductRatingsController(UseCaseHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Rate a product you purchased (or update your existing rating for it).
        /// </summary>
        /// <response code="204">Saved</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="422">Validation error</response>

        [HttpPost]
        [Authorize]
        public IActionResult Post([FromBody] RateProductDto data, [FromServices] IRateProductCommand command)
        {
            _handler.HandleCommand(command, data);
            return NoContent();
        }
    }
}
