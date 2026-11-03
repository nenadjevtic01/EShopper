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
    public class PaymentMethodsController : ControllerBase
    {
        private readonly UseCaseHandler _handler;

        public PaymentMethodsController(UseCaseHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Returns the available payment methods. Admins passing includeInactive=true also see deactivated ones.
        /// </summary>
        /// <response code="200">Returns payment methods</response>

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get([FromQuery] bool includeInactive, [FromServices] IGetPaymentMethodsQuery query)
        {
            return Ok(_handler.HandleQuery(query, includeInactive));
        }

        /// <summary>
        /// Admin: create a payment method.
        /// </summary>
        /// <response code="201">Created</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="422">Validation error</response>

        [HttpPost]
        [Authorize]
        public IActionResult Post([FromBody] CreatePaymentMethodDto data, [FromServices] ICreatePaymentMethodCommand command)
        {
            _handler.HandleCommand(command, data);
            return StatusCode(201);
        }

        /// <summary>
        /// Admin: update a payment method's name.
        /// </summary>
        /// <response code="204">Updated</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Not found</response>
        /// <response code="422">Validation error</response>

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult Put(int id, [FromBody] UpdatePaymentMethodDto data, [FromServices] IUpdatePaymentMethodCommand command)
        {
            _handler.HandleCommand(command, new UpdatePaymentMethodDto { Id = id, Name = data.Name });
            return NoContent();
        }

        /// <summary>
        /// Admin: toggle a payment method's active status.
        /// </summary>
        /// <response code="204">Toggled</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Not found</response>

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id, [FromServices] IDeletePaymentMethodCommand command)
        {
            _handler.HandleCommand(command, id);
            return NoContent();
        }
    }
}
