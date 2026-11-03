using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EShopper.Application.UseCases.Commands;
using EShopper.Implementation;

namespace EShopper.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class InitialController : ControllerBase
    {
        private UseCaseHandler _handler;

        public InitialController(UseCaseHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Initial insert to database
        /// </summary>
        /// <response code="204">Successfully inserted to database</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="422">Validation error</response>
        /// <response code="500">Unexpected Server Error</response>

        [HttpPost]
        public IActionResult Post([FromServices] IFillDatabaseCommand command)
        {
            _handler.HandleCommand(command, 0);
            return NoContent();
        }
    }
}
