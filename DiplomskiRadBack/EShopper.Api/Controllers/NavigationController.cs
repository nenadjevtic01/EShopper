using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EShopper.Application.UseCases.Queries;
using EShopper.Implementation;

namespace EShopper.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NavigationController : ControllerBase
    {
        private readonly UseCaseHandler _handler;

        public NavigationController(UseCaseHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Returns the list of top navigation links.
        /// </summary>
        /// <response code="200">Returns navigation links</response>

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get([FromServices] IGetNavigationQuery query)
        {
            return Ok(_handler.HandleQuery(query, 0));
        }
    }
}
