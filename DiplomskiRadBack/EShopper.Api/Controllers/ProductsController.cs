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
    public class ProductsController : ControllerBase
    {
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png" };
        private const long MaxImageSizeBytes = 5 * 1024 * 1024;
        private const int MaxImagesPerProduct = 5;

        private readonly UseCaseHandler _handler;
        private readonly IUser _user;
        private readonly IWebHostEnvironment _environment;
        private readonly EShopperDbContext _context;

        public ProductsController(UseCaseHandler handler, IUser user, IWebHostEnvironment environment, EShopperDbContext context)
        {
            _handler = handler;
            _user = user;
            _environment = environment;
            _context = context;
        }

        /// <summary>
        /// Returns filtered products
        /// </summary>
        /// <remarks>
        /// Sample request: GET /api/products?categoryid=2&amp;minimumprice=20&amp;maximumprice=150&amp;perPage=10
        /// </remarks>
        /// <response code="200">Returns filtered products</response>
        /// <response code="500">Unexpected server error.</response>

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get([FromQuery] SearchProductDto search, [FromServices] IGetProductQuery query)
        {
            return Ok(_handler.HandleQuery(query, search));
        }

        /// <summary>
        /// Returns single products.
        /// </summary>
        /// <remarks>
        /// Sample request: GET /api/products/5
        /// </remarks>
        /// <response code="200">Returns single product</response>
        /// <response code="500">Unexpected server error.</response>

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult Get(int id, [FromServices] IGetSingleProductQuery query)
        {
            return Ok(_handler.HandleQuery(query, id));
        }

        /// <summary>
        /// Add product
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     POST
        ///     {
        ///         "ProductName" : "Product 12" <br/>
        ///         "CategoryId" : 3 <br/>
        ///         "BrandId" : 2 <br/>
        ///         "GenderId" : 1 <br/>
        ///         "Sale" : true <br/>
        ///         "InStock" :true <br/>
        ///         "Material" : "Material 10" <br/>
        ///         "CountryOfOrigin" : "Country 2" <br/>
        ///         "Price" :
        ///         {
        ///             "Price" : 99,
        ///             "ActiveFrom" : "2023-06-07"
        ///         }<br/>
        ///         "Pictures" :
        ///         [
        ///         {
        ///             "Src":"slika1.jpg",
        ///             "Alt":"alt1.jpg"
        ///         },
        ///         {
        ///             "Src":"slika2.jpg",
        ///             "Alt":"alt2.jpg"
        ///         }
        ///         ],
        ///         "Sizes" : [1,2,3]
        ///     }
        /// </remarks>
        /// <response code="201">Successfully added</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="422">Validation error</response>
        /// <response code="500">Unexpected Server Error</response>

        [HttpPost]
        [Authorize]
        public IActionResult Post([FromBody] CreateProductDto product, [FromServices] ICreateProductCommand command)
        {
            _handler.HandleCommand(command, product);
            return NoContent();
        }

        /// <summary>
        /// Update product
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     PUT
        ///     {
        ///         "ProductId" : 3 <br/>
        ///         "ProductName" : "Product 12" <br/>
        ///         "CategoryId" : 3 <br/>
        ///         "BrandId" : 2 <br/>
        ///         "GenderId" : 1 <br/>
        ///         "Sale" : true <br/>
        ///         "InStock" :true <br/>
        ///         "Material" : "Material 10" <br/>
        ///         "CountryOfOrigin" : "Country 2" <br/>
        ///         "Pictures" :
        ///         [
        ///         {
        ///             "Src":"slika1.jpg",
        ///             "Alt":"alt1.jpg"
        ///         }
        ///         ],
        ///         "Sizes" : [1,2,3]
        ///     }
        /// </remarks>
        /// <response code="204">Successfully updated</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="422">Validation error</response>
        /// <response code="500">Unexpected Server Error</response>

        [HttpPut]
        [Authorize]
        public IActionResult Put([FromBody] UpdateProductDto product, [FromServices] IUpdateProductCommand command)
        {
            _handler.HandleCommand(command, product);
            return NoContent();
        }

        /// <summary>
        /// Admin: change the regular price of a product
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     POST /api/products/3/price
        ///     {
        ///         "Price" : 69.99
        ///     }
        /// The new price applies immediately. If the product is in a running or upcoming promotion,
        /// its sale price and the price it returns to after the promotion are recalculated from the new price.
        /// </remarks>
        /// <response code="204">Price changed</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="422">Validation error</response>
        /// <response code="500">Unexpected Server Error</response>

        [HttpPost("{id}/price")]
        [Authorize]
        public IActionResult PostPrice(int id, [FromBody] CreatePriceDto price, [FromServices] IAddProductPriceCommand command)
        {
            price.ProductId = id;
            _handler.HandleCommand(command, price);
            return NoContent();
        }

        /// <summary>
        /// Upload one or more product images
        /// </summary>
        /// <remarks>
        /// Sample request: POST /api/products/upload-images?category=jackets (multipart/form-data, field name "files")
        /// Handled directly in the controller (not through UseCaseHandler) because
        /// IFormFile content isn't meaningfully JSON-loggable like the other use cases.
        /// Files are written straight into the Angular project's own src/assets/img/{category}
        /// folder, where {category} is the category's folder name (its name in lowercase, spaces as hyphens) - the same place every existing product image already lives - so the
        /// returned path can be stored in Pictures.Src exactly like the seeded ones, no
        /// separate backend file host involved. Requires the Angular project (DiplomskiRadFront)
        /// to sit next to EShopper on disk, as it already does in this repo.
        /// </remarks>
        /// <response code="200">Returns the stored paths for the uploaded images</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="422">Validation error</response>
        /// <response code="500">Unexpected Server Error</response>

        [HttpPost("upload-images")]
        [Authorize]
        [RequestSizeLimit(20 * 1024 * 1024)]
        public IActionResult UploadImages(List<IFormFile> files, [FromQuery] string? category)
        {
            if (_user.Role == 2)
            {
                return Forbid();
            }

            if (files == null || files.Count == 0)
            {
                return UnprocessableEntity(new
                {
                    errors = new[]
                    {
                        new { property = "files", error = "At least one image file is required." }
                    }
                });
            }

            if (files.Count > MaxImagesPerProduct)
            {
                return UnprocessableEntity(new
                {
                    errors = new[]
                    {
                        new { property = "files", error = $"You can upload at most {MaxImagesPerProduct} images at a time." }
                    }
                });
            }

            var categoryFolder = category?.ToLowerInvariant();

            var categoryExists = CategoryFolder.IsValid(categoryFolder) && _context.Categories
                .Select(x => x.CategoryName)
                .AsEnumerable()
                .Any(x => CategoryFolder.FromName(x) == categoryFolder);

            if (!categoryExists)
            {
                return UnprocessableEntity(new
                {
                    errors = new[]
                    {
                        new { property = "category", error = "Choose an existing category before uploading images." }
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
                        new { property = "files", error = "Can't find the Angular project's assets folder next to the API project." }
                    }
                });
            }

            var assetsFolder = Path.Combine(assetsRoot, "img", categoryFolder);
            Directory.CreateDirectory(assetsFolder);

            var results = new List<string>();

            foreach (var file in files)
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!AllowedImageExtensions.Contains(extension))
                {
                    return UnprocessableEntity(new
                    {
                        errors = new[]
                        {
                            new { property = "files", error = $"Unsupported file type: {extension}. Allowed: {string.Join(", ", AllowedImageExtensions)}." }
                        }
                    });
                }

                if (file.Length == 0 || file.Length > MaxImageSizeBytes)
                {
                    return UnprocessableEntity(new
                    {
                        errors = new[]
                        {
                            new { property = "files", error = $"{file.FileName} must be between 1 byte and 5 MB." }
                        }
                    });
                }

                var fileName = $"{Guid.NewGuid():N}{extension}";
                var filePath = Path.Combine(assetsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                results.Add($"assets/img/{categoryFolder}/{fileName}");
            }

            return Ok(new { paths = results });
        }

        /// <summary>
        /// Delete product
        /// </summary>
        /// <remarks>
        /// Sample request: DELETE /api/products/id
        /// </remarks>
        /// <response code="204">Product deleted</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="422">Validation error</response>
        /// <response code="500">Unexpected Server Error</response>

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id, [FromServices] IDeleteProductCommand command)
        {
            _handler.HandleCommand(command, id);
            return NoContent();
        }
    }
}
