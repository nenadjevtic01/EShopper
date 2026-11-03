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
    public class BrandsController : ControllerBase
    {
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png" };
        private const long MaxImageSizeBytes = 5 * 1024 * 1024;

        private readonly UseCaseHandler _handler;
        private readonly IUser _user;
        private readonly IWebHostEnvironment _environment;

        public BrandsController(UseCaseHandler handler, IUser user, IWebHostEnvironment environment)
        {
            _handler = handler;
            _user = user;
            _environment = environment;
        }

        /// <summary>
        /// Returns filtered brands.
        /// </summary>
        /// <response code="200">Returns brands</response>

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get([FromQuery] PagedSearchDto search, [FromServices] IGetBrandQuery query)
        {
            return Ok(_handler.HandleQuery(query, search));
        }

        /// <summary>
        /// Add brand
        /// </summary>
        /// <response code="201">Successfully added brand</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="422">Validation error</response>

        [HttpPost]
        [Authorize]
        public IActionResult Post([FromBody] CreateBrandDto data, [FromServices] ICreateBrandCommand command)
        {
            _handler.HandleCommand(command, data);
            return StatusCode(201);
        }

        /// <summary>
        /// Update brand
        /// </summary>
        /// <response code="204">Brand updated</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="422">Validation error</response>

        [HttpPut]
        [Authorize]
        public IActionResult Put([FromBody] UpdateBrandDto data, [FromServices] IUpdateBrandCommand command)
        {
            _handler.HandleCommand(command, data);
            return NoContent();
        }

        /// <summary>
        /// Toggle brand active status (soft delete)
        /// </summary>
        /// <response code="204">Brand status toggled</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="409">Brand still has products assigned to it</response>

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id, [FromServices] IDeleteBrandCommand command)
        {
            _handler.HandleCommand(command, id);
            return NoContent();
        }

        /// <summary>
        /// Upload a brand image
        /// </summary>
        /// <remarks>
        /// Sample request: POST /api/Brands/upload-image (multipart/form-data, field name "file")
        /// Stores the file under the Angular project's src/assets/img/brands folder,
        /// same convention as product image uploads, just in its own dedicated folder.
        /// </remarks>
        /// <response code="200">Returns the stored path for the uploaded image</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="422">Validation error</response>
        /// <response code="500">Unexpected Server Error</response>

        [HttpPost("upload-image")]
        [Authorize]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public IActionResult UploadImage(IFormFile file)
        {
            if (_user.Role == 2)
            {
                return Forbid();
            }

            if (file == null || file.Length == 0)
            {
                return UnprocessableEntity(new { errors = new[] { new { property = "file", error = "An image file is required." } } });
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedImageExtensions.Contains(extension))
            {
                return UnprocessableEntity(new
                {
                    errors = new[]
                    {
                        new { property = "file", error = $"Unsupported file type: {extension}. Allowed: {string.Join(", ", AllowedImageExtensions)}." }
                    }
                });
            }

            if (file.Length > MaxImageSizeBytes)
            {
                return UnprocessableEntity(new
                {
                    errors = new[]
                    {
                        new { property = "file", error = $"{file.FileName} must be 5 MB or smaller." }
                    }
                });
            }

            var assetsRoot = AssetPaths.Root(_environment.ContentRootPath);

            if (!Directory.Exists(assetsRoot))
            {
                return StatusCode(500, new
                {
                    errors = new[]
                    {
                        new { property = "file", error = "Can't find the Angular project's assets folder next to the API project." }
                    }
                });
            }

            var assetsFolder = Path.Combine(assetsRoot, "img", "brands");
            Directory.CreateDirectory(assetsFolder);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(assetsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return Ok(new { path = $"assets/img/brands/{fileName}" });
        }
    }
}
