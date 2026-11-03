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
    public class CouponsController : ControllerBase
    {
        private readonly UseCaseHandler _handler;

        public CouponsController(UseCaseHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Admin: list coupons, with how many times each was used.
        /// </summary>
        /// <response code="200">Returns coupons</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>

        [HttpGet]
        [Authorize]
        public IActionResult Get([FromQuery] PagedSearchDto search, [FromServices] IGetCouponsQuery query)
        {
            return Ok(_handler.HandleQuery(query, search));
        }

        /// <summary>
        /// Admin: create a coupon.
        /// </summary>
        /// <response code="201">Created</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="409">A coupon with this code already exists</response>
        /// <response code="422">Validation error</response>

        [HttpPost]
        [Authorize]
        public IActionResult Post([FromBody] CreateCouponDto data, [FromServices] ICreateCouponCommand command)
        {
            _handler.HandleCommand(command, data);
            return StatusCode(201);
        }

        /// <summary>
        /// Admin: update a coupon.
        /// </summary>
        /// <response code="204">Updated</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Not found</response>
        /// <response code="409">A coupon with this code already exists</response>
        /// <response code="422">Validation error</response>

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult Put(int id, [FromBody] UpdateCouponDto data, [FromServices] IUpdateCouponCommand command)
        {
            _handler.HandleCommand(command, new UpdateCouponDto
            {
                Id = id,
                Code = data.Code,
                DiscountPercentage = data.DiscountPercentage,
                ValidUntil = data.ValidUntil
            });

            return NoContent();
        }

        /// <summary>
        /// Admin: toggle a coupon's active status.
        /// </summary>
        /// <response code="204">Toggled</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Not found</response>

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id, [FromServices] IDeleteCouponCommand command)
        {
            _handler.HandleCommand(command, id);
            return NoContent();
        }

        /// <summary>
        /// Check whether a coupon code can be used by the current user.
        /// </summary>
        /// <response code="200">Returns the validation result</response>
        /// <response code="401">Unauthorized</response>

        [HttpPost("validate")]
        [Authorize]
        public IActionResult Validate([FromBody] ValidateCouponDto data, [FromServices] IValidateCouponQuery query)
        {
            return Ok(_handler.HandleQuery(query, data));
        }
    }
}
