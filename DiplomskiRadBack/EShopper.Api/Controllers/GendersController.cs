using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EShopper.Application.UseCases.Queries;
using EShopper.Implementation;

namespace EShopper.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GendersController : ControllerBase
    {
        private readonly UseCaseHandler _handler;

        public GendersController(UseCaseHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Returns the list of genders, for the shop filter.
        /// </summary>
        /// <response code="200">Returns genders</response>

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get([FromServices] IGetGenderQuery query)
        {
            return Ok(_handler.HandleQuery(query, 0));
        }
    }
}
