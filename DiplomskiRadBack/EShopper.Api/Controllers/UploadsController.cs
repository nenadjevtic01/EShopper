using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EShopper.Application.UseCases.DTO;
using EShopper.Domain;
using EShopper.Implementation.Storage;

namespace EShopper.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadsController : ControllerBase
    {
        private static readonly Regex UploadedFile = new Regex(@"^assets/img/[a-z]+(-[a-z]+)*/(user-(?<owner>\d+)-)?[0-9a-f]{32}\.(jpg|jpeg|png|webp|mp4|webm)$");

        private readonly IUser _user;
        private readonly AssetCleaner _cleaner;

        public UploadsController(IUser user, AssetCleaner cleaner)
        {
            _user = user;
            _cleaner = cleaner;
        }

        /// <summary>
        /// Removes files that were uploaded but never saved on any record (for example when a form is cancelled).
        /// </summary>
        /// <remarks>
        /// POST /api/Uploads/discard with a JSON body { "paths": ["assets/img/..."] }.
        /// Only files that are not used by any record are deleted; avatars can only be discarded by their owner.
        /// </remarks>
        /// <response code="204">Files discarded</response>
        /// <response code="401">Unauthorized</response>

        [HttpPost("discard")]
        [Authorize]
        public IActionResult Discard([FromBody] DiscardUploadsDto data)
        {
            foreach (var path in data.Paths ?? new List<string>())
            {
                var match = UploadedFile.Match(AssetCleaner.Normalize(path));

                if (!match.Success)
                {
                    continue;
                }

                var owner = match.Groups["owner"];
                var allowed = owner.Success ? owner.Value == _user.Id.ToString() : _user.Role != 2;

                if (allowed)
                {
                    _cleaner.DeleteIfUnused(path);
                }
            }

            return NoContent();
        }
    }
}
