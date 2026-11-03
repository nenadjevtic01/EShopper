using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EShopper.Application.UseCases.Queries;
using EShopper.Implementation;

namespace EShopper.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SizesController : ControllerBase
    {
        private readonly UseCaseHandler _handler;

        public SizesController(UseCaseHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Returns the list of sizes, for the shop filter.
        /// </summary>
        /// <response code="200">Returns sizes</response>

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get([FromServices] IGetSizeQuery query)
        {
            return Ok(_handler.HandleQuery(query, 0));
        }
    }
}
