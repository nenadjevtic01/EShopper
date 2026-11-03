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
    public class PromotionsController : ControllerBase
    {
        private readonly UseCaseHandler _handler;

        public PromotionsController(UseCaseHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Admin: list promotions with their discount groups and products.
        /// </summary>
        /// <response code="200">Returns promotions</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>

        [HttpGet]
        [Authorize]
        public IActionResult Get([FromQuery] PagedSearchDto search, [FromServices] IGetPromotionsQuery query)
        {
            return Ok(_handler.HandleQuery(query, search));
        }

        /// <summary>
        /// Public: promotions whose dates include today (and that are switched on), with their discounted products, for the home page.
        /// </summary>
        /// <response code="200">Returns active promotions</response>

        [HttpGet("active")]
        public IActionResult GetActive([FromServices] IGetActivePromotionsQuery query)
        {
            return Ok(_handler.HandleQuery(query, 0));
        }

        /// <summary>
        /// Admin: create a promotion with one or more discount groups of products.
        /// </summary>
        /// <response code="201">Created</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="422">Validation error</response>

        [HttpPost]
        [Authorize]
        public IActionResult Post([FromBody] CreatePromotionDto data, [FromServices] ICreatePromotionCommand command)
        {
            _handler.HandleCommand(command, data);
            return StatusCode(201);
        }

        /// <summary>
        /// Admin: replace a promotion's name and discount groups.
        /// </summary>
        /// <response code="204">Updated</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Not found</response>
        /// <response code="422">Validation error</response>

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult Put(int id, [FromBody] UpdatePromotionDto data, [FromServices] IUpdatePromotionCommand command)
        {
            _handler.HandleCommand(command, new UpdatePromotionDto
            {
                Id = id,
                Name = data.Name,
                ValidFrom = data.ValidFrom,
                ValidUntil = data.ValidUntil,
                Groups = data.Groups
            });

            return NoContent();
        }

        /// <summary>
        /// Admin: switch a promotion off or on (IsActive). An expired promotion can not be switched on again.
        /// </summary>
        /// <response code="204">Toggled</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Not found</response>
        /// <response code="422">Expired promotion can not be activated</response>

        [HttpPut("{id}/toggle")]
        [Authorize]
        public IActionResult Toggle(int id, [FromServices] ITogglePromotionCommand command)
        {
            _handler.HandleCommand(command, id);
            return NoContent();
        }

        /// <summary>
        /// Admin: delete a promotion. Its scheduled prices are removed and, if it was running, products return to their regular prices.
        /// </summary>
        /// <response code="204">Deleted</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Not found</response>

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id, [FromServices] IDeletePromotionCommand command)
        {
            _handler.HandleCommand(command, id);
            return NoContent();
        }
    }
}
