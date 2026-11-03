using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.Application.UseCases.Queries;
using EShopper.Implementation;

namespace EShopper.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsletterController : ControllerBase
    {
        private readonly UseCaseHandler _handler;

        public NewsletterController(UseCaseHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Subscribe an e-mail address to the newsletter.
        /// </summary>
        /// <response code="204">Subscribed</response>
        /// <response code="422">Validation error</response>

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Post([FromBody] SubscribeNewsletterDto data, [FromServices] ISubscribeNewsletterCommand command)
        {
            _handler.HandleCommand(command, data);
            return NoContent();
        }

        /// <summary>
        /// Admin: list newsletter subscribers.
        /// </summary>
        /// <response code="200">Returns subscribers</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>

        [HttpGet]
        [Authorize]
        public IActionResult Get([FromQuery] PagedSearchDto search, [FromServices] IGetNewsletterSubscribersQuery query)
        {
            return Ok(_handler.HandleQuery(query, search));
        }
    }
}
