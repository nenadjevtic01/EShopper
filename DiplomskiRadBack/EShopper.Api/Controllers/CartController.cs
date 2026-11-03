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
    public class CartController : ControllerBase
    {
        private readonly UseCaseHandler _handler;

        public CartController(UseCaseHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Returns single cart.
        /// </summary>
        /// <remarks>
        /// Sample request: GET /api/cart/id
        /// </remarks>
        /// <response code="200">Returns single cart</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult Get(int id, [FromServices] IGetCartQuery query)
        {
            return Ok(_handler.HandleQuery(query, id));
        }

        /// <summary>
        /// Add to cart
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     POST
        ///     {
        ///         "CartId" : 3 <br/>
        ///         "ProductId" : 3 <br/>
        ///         "SizeId" : 2 <br/>
        ///         "Quantity" : 10 <br/>
        ///     }
        /// </remarks>
        /// <response code="201">Successfully added to cart</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="422">Validation error</response>
        /// <response code="500">Unexpected Server Error</response>

        [HttpPost]
        [Authorize]
        public IActionResult Post([FromBody] CreateCartItemDto data, [FromServices] ICreateCartItemCommand command)
        {
            _handler.HandleCommand(command, data);
            return NoContent();
        }

        /// <summary>
        /// Update cart item
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     PUT
        ///     {
        ///         "CartItemId" : 3 <br/>
        ///         "CartId": 2 <br/>
        ///         "ProductId" : 3 <br/>
        ///         "SizeId" : 2 <br/>
        ///         "Quantity" : 10 <br/>
        ///     }
        /// </remarks>
        /// <response code="204">Cart item updated</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="422">Validation error</response>
        /// <response code="500">Unexpected Server Error</response>
        [HttpPut]
        [Authorize]
        public IActionResult Put([FromBody] UpdateCartItemDto data, [FromServices] IUpdateCartItemCommand command)
        {
            _handler.HandleCommand(command, data);
            return NoContent();
        }

        /// <summary>
        /// Delete cart item
        /// </summary>
        /// <remarks>
        /// Sample request: DELETE /api/cart/id
        /// </remarks>
        /// <response code="204">Cart item deleted</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="422">Validation error</response>
        /// <response code="500">Unexpected Server Error</response>

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id, [FromServices] IDeleteCartItemCommand command)
        {
            _handler.HandleCommand(command, id);
            return NoContent();
        }
    }
}
