using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EShopper.Application.UseCases.DTO;
using EShopper.Application.UseCases.Queries;
using EShopper.Implementation;

namespace EShopper.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogsController : ControllerBase
    {
        private readonly UseCaseHandler _handler;

        public AuditLogsController(UseCaseHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Admin: returns executed use cases (audit log), newest first.
        /// </summary>
        /// <remarks>
        /// Sample request: GET /api/AuditLogs?Keyword=product&amp;IsAuthorized=false&amp;Page=1&amp;PerPage=15
        /// Values of password and token fields are masked.
        /// </remarks>
        /// <response code="200">Returns a page of audit log entries</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>

        [HttpGet]
        [Authorize]
        public IActionResult Get([FromQuery] SearchAuditLogDto search, [FromServices] IGetAuditLogsQuery query)
        {
            return Ok(_handler.HandleQuery(query, search));
        }
    }
}
