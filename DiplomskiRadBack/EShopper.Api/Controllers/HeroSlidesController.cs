using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EShopper.Api.Core;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.Application.UseCases.Queries;
using EShopper.Domain;
using EShopper.Implementation;

namespace EShopper.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeroSlidesController : ControllerBase
    {
        private static readonly string[] ImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private static readonly string[] VideoExtensions = { ".mp4", ".webm" };
        private const long MaxImageSizeBytes = 8 * 1024 * 1024;
        private const long MaxVideoSizeBytes = 50 * 1024 * 1024;

        private readonly UseCaseHandler _handler;
        private readonly IUser _user;
        private readonly IWebHostEnvironment _environment;

        public HeroSlidesController(UseCaseHandler handler, IUser user, IWebHostEnvironment environment)
        {
            _handler = handler;
            _user = user;
            _environment = environment;
        }

        /// <summary>
        /// Returns the home page hero slides. Admins passing includeInactive=true also see deactivated ones.
        /// </summary>
        /// <response code="200">Returns hero slides</response>

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get([FromQuery] bool includeInactive, [FromServices] IGetHeroSlidesQuery query)
        {
            return Ok(_handler.HandleQuery(query, includeInactive));
        }

        /// <summary>
        /// Admin: create a hero slide.
        /// </summary>
        /// <response code="201">Created</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="422">Validation error</response>

        [HttpPost]
        [Authorize]
        public IActionResult Post([FromBody] CreateHeroSlideDto data, [FromServices] ICreateHeroSlideCommand command)
        {
            _handler.HandleCommand(command, data);
            return StatusCode(201);
        }

        /// <summary>
        /// Admin: update a hero slide.
        /// </summary>
        /// <response code="204">Updated</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Not found</response>
        /// <response code="422">Validation error</response>

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult Put(int id, [FromBody] UpdateHeroSlideDto data, [FromServices] IUpdateHeroSlideCommand command)
        {
            data.Id = id;
            _handler.HandleCommand(command, data);
            return NoContent();
        }

        /// <summary>
        /// Admin: toggle a hero slide active status.
        /// </summary>
        /// <response code="204">Toggled</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Not found</response>

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id, [FromServices] IDeleteHeroSlideCommand command)
        {
            _handler.HandleCommand(command, id);
            return NoContent();
        }

        /// <summary>
        /// Admin: upload a hero image (jpg, png, webp - 8 MB) or video (mp4, webm - 50 MB).
        /// </summary>
        /// <remarks>
        /// POST /api/HeroSlides/upload (multipart/form-data, field name "file").
        /// Images and videos are stored in src/assets/img/hero in the Angular project.
        /// </remarks>
        /// <response code="200">Returns the stored path and the detected media type</response>
        /// <response code="403">Forbidden</response>
        /// <response code="422">Validation error</response>

        [HttpPost("upload")]
        [Authorize]
        [RequestSizeLimit(52 * 1024 * 1024)]
        public IActionResult Upload(IFormFile file)
        {
            if (_user.Role != 1)
            {
                return Forbid();
            }

            if (file == null || file.Length == 0)
            {
                return UnprocessableEntity(Error("A file is required."));
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            string mediaType;
            long limit;

            if (ImageExtensions.Contains(extension))
            {
                mediaType = "image";
                limit = MaxImageSizeBytes;
            }
            else if (VideoExtensions.Contains(extension))
            {
                mediaType = "video";
                limit = MaxVideoSizeBytes;
            }
            else
            {
                return UnprocessableEntity(Error($"Unsupported file type: {extension}. Allowed: {string.Join(", ", ImageExtensions.Concat(VideoExtensions))}."));
            }

            if (file.Length > limit)
            {
                return UnprocessableEntity(Error($"{file.FileName} must be {limit / (1024 * 1024)} MB or smaller."));
            }

            var assetsRoot = AssetPaths.Root(_environment.ContentRootPath);

            if (!Directory.Exists(assetsRoot))
            {
                return StatusCode(500, Error("Can not find the Angular project assets folder next to the API project."));
            }

            var targetFolder = Path.Combine(assetsRoot, "img", "hero");
            Directory.CreateDirectory(targetFolder);

            var fileName = $"{Guid.NewGuid():N}{extension}";

            using (var stream = new FileStream(Path.Combine(targetFolder, fileName), FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return Ok(new { path = $"assets/img/hero/{fileName}", mediaType });
        }

        private static object Error(string message)
        {
            return new { errors = new[] { new { property = "file", error = message } } };
        }
    }
}
