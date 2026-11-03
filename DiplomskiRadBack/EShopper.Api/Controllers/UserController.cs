using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EShopper.Api.Core;
using EShopper.Api.Core.DTO;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.Application.UseCases.Queries;
using EShopper.Domain;
using EShopper.Implementation;

namespace EShopper.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly JwtManager _manager;
        private static readonly string[] AllowedAvatarExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxAvatarSizeBytes = 2 * 1024 * 1024;

        private readonly UseCaseHandler _handler;
        private readonly IUser _user;
        private readonly IWebHostEnvironment _environment;

        public UserController(JwtManager manager, UseCaseHandler handler, IUser user, IWebHostEnvironment environment)
        {
            _manager = manager;
            _handler = handler;
            _user = user;
            _environment = environment;
        }

        /// <summary>
        /// Returns users with provided id.
        /// </summary>
        /// <remarks>
        /// Sample request: GET /api/users/1
        /// </remarks>
        /// <response code="200">Returns user</response>
        /// <response code="500">Unexpected server error.</response>

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetUser(int id, [FromServices] IGetSingleUserQuery query)
        {
            return Ok(_handler.HandleQuery(query, id));
        }

        /// <summary>
        /// Returns filtered users.
        /// </summary>
        /// <remarks>
        /// Sample request: GET /api/users?keyword=c&amp;perPage=10
        /// </remarks>
        /// <response code="200">Returns filtered users</response>
        /// <response code="500">Unexpected server error.</response>

        [HttpGet]
        [Authorize]
        public IActionResult Get([FromQuery] PagedSearchDto search, [FromServices] IGetUserQuery query)
        {
            return Ok(_handler.HandleQuery(query, search));
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     POST
        ///     {
        ///         "Email" : "user@gmail.com" <br/>
        ///         "Password" : "Sifra123"
        ///     }
        /// </remarks>
        /// <response code="200">Token generated</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Unexpected Server Error</response>

        [HttpPost("/api/token")]
        [AllowAnonymous]
        public IActionResult Token([FromBody] TokenRequestDto request)
        {
            try
            {
                var token = _manager.MakeToken(request.Email, request.Password);

                return Ok(new { token });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Add user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     POST
        ///     {
        ///         "FirstName" : "Mika" <br/>
        ///         "LastName" : "Mikic" <br/>
        ///         "Email" : "mikamikic@gmail.com" <br/>
        ///         "Password" : "Sifra123" <br/>
        ///         "UserName" : "mikamikic123" <br/>
        ///         "Address" : "Adresa 1" <br/>
        ///         "City" : "Grad" <br/>
        ///         "PostalCode" : 11000 <br/>
        ///         "Country" : "Srbija"
        ///     }
        /// </remarks>
        /// <response code="201">Successfully added user</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="422">Validation error</response>
        /// <response code="500">Unexpected Server Error</response>

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Post([FromBody] CreateUserDto user, [FromServices] ICreateUserCommand command)
        {
            _handler.HandleCommand(command, user);
            return NoContent();
        }

        /// <summary>
        /// Update user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     PUT
        ///     {
        ///         "UserId" : 3 <br/>
        ///         "Address": "Adresa 2" <br/>
        ///         "City" : "Grad 2" <br/>
        ///         "PostalCode" : 11200 <br/>
        ///         "Country" : "Drzava 2" <br/>
        ///     }
        /// </remarks>
        /// <response code="204">User updated</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="422">Validation error</response>
        /// <response code="500">Unexpected Server Error</response>

        [HttpPut]
        [Authorize]
        public IActionResult Put([FromBody] UpdateUserInfoDto data, [FromServices] IUpdateUserInfoCommand command)
        {
            _handler.HandleCommand(command, data);
            return NoContent();
        }

        /// <summary>
        /// Sets or removes the avatar of the logged in user.
        /// </summary>
        /// <remarks>
        /// Sample request: PUT /api/User/avatar (an empty Src removes the avatar)
        /// </remarks>
        /// <response code="204">Avatar updated</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="422">Validation error</response>
        /// <response code="500">Unexpected Server Error</response>

        [HttpPut("avatar")]
        [Authorize]
        public IActionResult PutAvatar([FromBody] UpdateAvatarDto data, [FromServices] IUpdateAvatarCommand command)
        {
            _handler.HandleCommand(command, data);
            return NoContent();
        }

        /// <summary>
        /// Upload an avatar image for the logged in user
        /// </summary>
        /// <remarks>
        /// Sample request: POST /api/User/upload-avatar (multipart/form-data, field name "file")
        /// Stores the file under the Angular project's src/assets/img/avatars folder as user-{id}-{random}.{extension}.
        /// </remarks>
        /// <response code="200">Returns the stored path for the uploaded image</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="422">Validation error</response>
        /// <response code="500">Unexpected Server Error</response>

        [HttpPost("upload-avatar")]
        [Authorize]
        [RequestSizeLimit(4 * 1024 * 1024)]
        public IActionResult UploadAvatar(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return UnprocessableEntity(new { errors = new[] { new { property = "file", error = "An image file is required." } } });
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedAvatarExtensions.Contains(extension))
            {
                return UnprocessableEntity(new
                {
                    errors = new[]
                    {
                        new { property = "file", error = $"Unsupported file type: {extension}. Allowed: {string.Join(", ", AllowedAvatarExtensions)}." }
                    }
                });
            }

            if (file.Length > MaxAvatarSizeBytes)
            {
                return UnprocessableEntity(new
                {
                    errors = new[]
                    {
                        new { property = "file", error = $"{file.FileName} must be 2 MB or smaller." }
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

            var avatarFolder = Path.Combine(assetsRoot, "img", "avatars");
            Directory.CreateDirectory(avatarFolder);

            var fileName = $"user-{_user.Id}-{Guid.NewGuid():N}{extension}";

            using (var stream = new FileStream(Path.Combine(avatarFolder, fileName), FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return Ok(new { path = $"assets/img/avatars/{fileName}" });
        }

        /// <summary>
        /// Admin creates a user (no password set here - the invitee sets it via emailed link)
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     POST
        ///     {
        ///         "FirstName" : "Mika" <br/>
        ///         "LastName" : "Mikic" <br/>
        ///         "Email" : "mikamikic@gmail.com" <br/>
        ///         "UserName" : "mikamikic123" <br/>
        ///         "Address" : "Adresa 1" <br/>
        ///         "City" : "Grad" <br/>
        ///         "PostalCode" : 11000 <br/>
        ///         "Country" : "Srbija"
        ///     }
        /// </remarks>
        /// <response code="200">Returns the created user id and the set-password link</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="422">Validation error</response>
        /// <response code="500">Unexpected Server Error</response>

        [HttpPost("admin-create")]
        [Authorize]
        public IActionResult AdminCreate([FromBody] AdminCreateUserDto user, [FromServices] IAdminCreateUserQuery query)
        {
            return Ok(_handler.HandleQuery(query, user));
        }

        /// <summary>
        /// Request a password reset link by email
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     POST
        ///     {
        ///         "Email" : "user@gmail.com"
        ///     }
        /// Always responds the same way regardless of whether the email is registered,
        /// so the endpoint can't be used to enumerate accounts.
        /// </remarks>
        /// <response code="204">Request processed</response>
        /// <response code="500">Unexpected Server Error</response>

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordDto data, [FromServices] IForgotPasswordCommand command)
        {
            _handler.HandleCommand(command, data);
            return NoContent();
        }

        /// <summary>
        /// Set a new password using a valid reset token
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     POST
        ///     {
        ///         "Token" : "abc123..." <br/>
        ///         "NewPassword" : "Sifra123"
        ///     }
        /// </remarks>
        /// <response code="204">Password updated</response>
        /// <response code="422">Validation error (invalid/expired token or weak password)</response>
        /// <response code="500">Unexpected Server Error</response>

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public IActionResult ResetPassword([FromBody] ResetPasswordDto data, [FromServices] IResetPasswordCommand command)
        {
            _handler.HandleCommand(command, data);
            return NoContent();
        }

        /// <summary>
        /// Delete (ban) user
        /// </summary>
        /// <remarks>
        /// Sample request: DELETE /api/user/id
        /// </remarks>
        /// <response code="204">User deleted</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="422">Validation error</response>
        /// <response code="500">Unexpected Server Error</response>

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id, [FromServices] IDeleteUserCommand command)
        {
            _handler.HandleCommand(command, id);
            return NoContent();
        }
    }
}
