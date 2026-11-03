using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Results;
using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;
using EShopper.Domain;
using EShopper.Implementation.Storage;

namespace EShopper.Implementation.UseCases.Commands
{
    public class UpdateAvatarCommandEf : UseCaseEf, IUpdateAvatarCommand
    {
        private const string AvatarFolder = "assets/img/avatars/";

        private readonly IUser _user;
        private readonly AssetCleaner _cleaner;

        public int UseCaseId => 62;

        public string UseCaseName => "Update avatar";

        public string UseCaseDescription => "Set or remove user avatar.";

        public UpdateAvatarCommandEf(EShopperDbContext context, IUser user, AssetCleaner cleaner) : base(context)
        {
            _user = user;
            _cleaner = cleaner;
        }

        public void Execute(UpdateAvatarDto request)
        {
            var user = Context.Users.FirstOrDefault(x => x.Id == _user.Id);

            if (user == null)
            {
                throw new EntityNotExistException(nameof(User), _user.Id);
            }

            var src = (request.Src ?? "").Trim();

            if (src.Length > 0 && !Regex.IsMatch(src, "^" + AvatarFolder + "user-" + user.Id + "-[0-9a-f]{32}\\.(jpg|jpeg|png|webp)$"))
            {
                throw new ValidationException(new[] { new ValidationFailure("Src", "Avatar must be an uploaded image.") });
            }

            var oldSrc = user.AvatarSrc ?? "";
            user.AvatarSrc = src.Length == 0 ? null : src;
            Context.Users.Update(user);
            Context.SaveChanges();

            if (oldSrc.Length > 0 && oldSrc != src)
            {
                _cleaner.DeleteIfUnused(oldSrc);
            }
        }
    }
}
