using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Commands
{
    public class CreateHeroSlideCommandEf : UseCaseEf, ICreateHeroSlideCommand
    {
        private readonly IUser _user;

        public int UseCaseId => 59;

        public string UseCaseName => "Create hero slide";

        public string UseCaseDescription => "Create new hero slide.";

        public CreateHeroSlideCommandEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public void Execute(CreateHeroSlideDto request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User does not have permission to create hero slides.");
            }

            HeroSlideRules.Validate(request);

            var slide = new HeroSlide();
            HeroSlideRules.Apply(request, slide);
            Context.HeroSlides.Add(slide);
            Context.SaveChanges();
        }
    }
}
