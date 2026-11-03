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
    public class ContactMessagesController : ControllerBase
    {
        private readonly UseCaseHandler _handler;

        public ContactMessagesController(UseCaseHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Submit a contact form message.
        /// </summary>
        /// <response code="201">Message submitted</response>
        /// <response code="422">Validation error</response>

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Post([FromBody] CreateContactMessageDto data, [FromServices] ICreateContactMessageCommand command)
        {
            _handler.HandleCommand(command, data);
            return StatusCode(201);
        }

        /// <summary>
        /// Admin: list submitted contact messages.
        /// </summary>
        /// <response code="200">Returns messages</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>

        [HttpGet]
        [Authorize]
        public IActionResult Get([FromQuery] SearchContactMessageDto search, [FromServices] IGetContactMessagesQuery query)
        {
            return Ok(_handler.HandleQuery(query, search));
        }

        /// <summary>
        /// Admin: answer a contact message.
        /// </summary>
        /// <response code="204">Answer saved</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="422">Validation error</response>

        [HttpPut("{id}/answer")]
        [Authorize]
        public IActionResult Answer(int id, [FromBody] AnswerContactMessageDto data, [FromServices] IAnswerContactMessageCommand command)
        {
            _handler.HandleCommand(command, new AnswerContactMessageDto { Id = id, Answer = data.Answer });
            return NoContent();
        }

        /// <summary>
        /// Admin: toggle a message's active status (soft delete/restore).
        /// </summary>
        /// <response code="204">Message status toggled</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id, [FromServices] IDeleteContactMessageCommand command)
        {
            _handler.HandleCommand(command, id);
            return NoContent();
        }
    }
}
