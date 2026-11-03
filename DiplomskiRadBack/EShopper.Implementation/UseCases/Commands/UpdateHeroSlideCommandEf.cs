using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;
using EShopper.Domain;
using EShopper.Implementation.Storage;

namespace EShopper.Implementation.UseCases.Commands
{
    public class UpdateHeroSlideCommandEf : UseCaseEf, IUpdateHeroSlideCommand
    {
        private readonly IUser _user;
        private readonly AssetCleaner _cleaner;

        public int UseCaseId => 60;

        public string UseCaseName => "Update hero slide";

        public string UseCaseDescription => "Update hero slide details.";

        public UpdateHeroSlideCommandEf(EShopperDbContext context, IUser user, AssetCleaner cleaner) : base(context)
        {
            _user = user;
            _cleaner = cleaner;
        }

        public void Execute(UpdateHeroSlideDto request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User does not have permission to update hero slides.");
            }

            var slide = Context.HeroSlides.FirstOrDefault(x => x.Id == request.Id);

            if (slide == null)
            {
                throw new EntityNotExistException(nameof(HeroSlide), request.Id);
            }

            HeroSlideRules.Validate(request);

            var oldSrc = slide.MediaSrc ?? "";
            HeroSlideRules.Apply(request, slide);
            Context.HeroSlides.Update(slide);
            Context.SaveChanges();

            if (oldSrc.Length > 0 && oldSrc != (slide.MediaSrc ?? ""))
            {
                _cleaner.DeleteIfUnused(oldSrc);
            }
        }
    }
}
