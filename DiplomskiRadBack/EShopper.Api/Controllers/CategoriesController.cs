using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EShopper.Api.Core;
using EShopper.Application.Storage;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.Application.UseCases.Queries;
using EShopper.DataAccess;
using EShopper.Domain;
using EShopper.Implementation;

namespace EShopper.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png" };
        private const long MaxImageSizeBytes = 5 * 1024 * 1024;

        private UseCaseHandler _handler;
        private readonly IUser _user;
        private readonly IWebHostEnvironment _environment;
        private readonly EShopperDbContext _context;

        public CategoriesController(UseCaseHandler handler, IUser user, IWebHostEnvironment environment, EShopperDbContext context)
        {
            _handler = handler;
            _user = user;
            _environment = environment;
            _context = context;
        }

        /// <summary>
        /// Returns filtered categories.
        /// </summary>
        /// <remarks>
        /// Sample request: GET /api/categories?keyword=c&amp;perPage=10
        /// </remarks>
        /// <response code="200">Returns categories</response>
        /// <response code="500">Unexpected server error.</response>

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get([FromQuery] PagedSearchDto search, [FromServices] IGetCategoryQuery query)
        {
            return Ok(_handler.HandleQuery(query, search));
        }

        /// <summary>
        /// Add category
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     POST
        ///     {
        ///         "Name" : "Category 1" <br/>
        ///     }
        /// </remarks>
        /// <response code="201">Successfully added category</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="422">Validation error</response>
        /// <response code="500">Unexpected Server Error</response>

        [HttpPost]
        [Authorize]
        public IActionResult Post([FromBody] CreateCategoryDto data, [FromServices] ICreateCategoryCommand command)
        {
            _handler.HandleCommand(command, data);

            return StatusCode(201);
        }

        /// <summary>
        /// Update category
        /// </summary>
        /// <response code="204">Category updated</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="422">Validation error</response>

        [HttpPut]
        [Authorize]
        public IActionResult Put([FromBody] UpdateCategoryDto data, [FromServices] IUpdateCategoryCommand command)
        {
            _handler.HandleCommand(command, data);
            return NoContent();
        }

        /// <summary>
        /// Toggle category active status (soft delete)
        /// </summary>
        /// <response code="204">Category status toggled</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="409">Category still has products assigned to it</response>

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id, [FromServices] IDeleteCategoryCommand command)
        {
            _handler.HandleCommand(command, id);
            return NoContent();
        }

        /// <summary>
        /// Upload a category image
        /// </summary>
        /// <remarks>
        /// Sample request: POST /api/categories/upload-image?name=Coats (multipart/form-data, field name "file")
        /// Stores the file in the category's own folder under the Angular project's src/assets/img
        /// (for example assets/img/coats), the same folder that holds the images of its products.
        /// When editing an existing category, pass categoryId too so its own name is not reported as taken.
        /// </remarks>
        /// <response code="200">Returns the stored path for the uploaded image</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="422">Validation error</response>
        /// <response code="500">Unexpected Server Error</response>

        [HttpPost("upload-image")]
        [Authorize]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public IActionResult UploadImage(IFormFile file, [FromQuery] string? name, [FromQuery] int? categoryId)
        {
            if (_user.Role == 2)
            {
                return Forbid();
            }

            var categoryName = CategoryFolder.CleanName(name);

            if (!Regex.IsMatch(categoryName, @"^[A-Za-z ]{3,25}$") || CategoryFolder.IsReserved(categoryName))
            {
                return UnprocessableEntity(new { errors = new[] { new { property = "name", error = "Enter a valid category name before uploading its image." } } });
            }

            if (_context.Categories.Any(x => x.CategoryName == categoryName && x.Id != categoryId))
            {
                return UnprocessableEntity(new { errors = new[] { new { property = "name", error = "Category name already exists." } } });
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

            var folder = CategoryFolder.FromName(categoryName);
            var assetsFolder = Path.Combine(assetsRoot, "img", folder);
            Directory.CreateDirectory(assetsFolder);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(assetsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return Ok(new { path = $"assets/img/{folder}/{fileName}" });
        }
    }
}
