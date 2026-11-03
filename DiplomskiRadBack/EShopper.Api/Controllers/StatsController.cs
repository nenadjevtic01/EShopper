using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EShopper.Application.UseCases.Queries;
using EShopper.Implementation;

namespace EShopper.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly UseCaseHandler _handler;

        public StatsController(UseCaseHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Admin dashboard stats: orders/revenue per month, sales share by category and brand.
        /// </summary>
        /// <response code="200">Returns stats</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>

        [HttpGet]
        [Authorize]
        public IActionResult Get([FromServices] IGetAdminStatsQuery query)
        {
            return Ok(_handler.HandleQuery(query, 0));
        }
    }
}
