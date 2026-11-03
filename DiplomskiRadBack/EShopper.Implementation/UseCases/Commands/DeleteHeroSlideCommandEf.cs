using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.Commands;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Commands
{
    public class DeleteHeroSlideCommandEf : UseCaseEf, IDeleteHeroSlideCommand
    {
        private readonly IUser _user;

        public int UseCaseId => 61;

        public string UseCaseName => "Delete hero slide";

        public string UseCaseDescription => "Toggle hero slide active status.";

        public DeleteHeroSlideCommandEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public void Execute(int request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User does not have permission to delete hero slides.");
            }

            var slide = Context.HeroSlides.FirstOrDefault(x => x.Id == request);

            if (slide == null)
            {
                throw new EntityNotExistException(nameof(HeroSlide), request);
            }

            slide.IsActive = !slide.IsActive;
            Context.HeroSlides.Update(slide);
            Context.SaveChanges();
        }
    }
}
