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
    public class ReceiptController : ControllerBase
    {
        private UseCaseHandler _handler;

        public ReceiptController(UseCaseHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Filter receipts
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     GET
        ///     {
        ///         "FromDate" : "2023-06-06" <br/>
        ///         "ToDate" : "2023-06-08" <br/>
        ///         "UserId" : 3 <br/>
        ///     }
        /// </remarks>
        /// <response code="200">Returns filtered receipts</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="422">Validation error</response>
        /// <response code="500">Unexpected Server Error</response>

        [HttpGet]
        [Authorize]
        public IActionResult Get([FromQuery] SearchReceiptDto data, [FromServices] IGetReceiptsQuery query)
        {
            return Ok(_handler.HandleQuery(query, data));
        }

        /// <summary>
        /// Filter receipts
        /// </summary>
        /// <remarks>
        /// Sample request: GET /api/receipt/id
        /// </remarks>
        /// <response code="200">Returns single receipt</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="422">Validation error</response>
        /// <response code="500">Unexpected Server Error</response>
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult Get(int id, [FromServices] IGetSingleReceiptQuery query)
        {
            return Ok(_handler.HandleQuery(query, id));
        }

        /// <summary>
        /// Confirms order
        /// </summary>
        /// <remarks>
        /// Sample request: POST /api/receipt/id
        ///     {
        ///         "PaymentMethodId" : 1
        ///     }
        /// </remarks>
        /// <response code="200">Order confirmed, returns the new receipt id</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="422">Validation error</response>
        /// <response code="500">Unexpected Server Error</response>

        [HttpPost("{id}")]
        [Authorize]
        public IActionResult Post(int id, [FromBody] ConfirmOrderDto data, [FromServices] IConfirmOrderCommand command)
        {
            var receiptId = _handler.HandleQuery(command, new ConfirmOrderDto
            {
                UserId = id,
                PaymentMethodId = data.PaymentMethodId,
                BillingFirstName = data.BillingFirstName,
                BillingLastName = data.BillingLastName,
                BillingEmail = data.BillingEmail,
                BillingAddress = data.BillingAddress,
                BillingCity = data.BillingCity,
                BillingCountry = data.BillingCountry,
                BillingPostalCode = data.BillingPostalCode,
                CouponCode = data.CouponCode
            });

            return Ok(new { receiptId });
        }

        /// <summary>
        /// Deletes receipts
        /// </summary>
        /// <remarks>
        /// Sample request: DELETE /api/receipt/id
        /// </remarks>
        /// <response code="204"> Receipt deleted</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="422">Validation error</response>
        /// <response code="500">Unexpected Server Error</response>

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id, [FromServices] IDeleteReceiptCommand command)
        {
            _handler.HandleCommand(command, id);
            return NoContent();
        }
    }
}
