using EShopper.Application.UseCases.DTO;
using EShopper.Application.UseCases.Queries;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Queries
{
    public class GetHeroSlidesQueryEf : UseCaseEf, IGetHeroSlidesQuery
    {
        private readonly IUser _user;

        public int UseCaseId => 58;

        public string UseCaseName => "Get hero slides";

        public string UseCaseDescription => "Get slides for home page hero.";

        public GetHeroSlidesQueryEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public IEnumerable<HeroSlideDto> Execute(bool request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => (int?)x.Role).FirstOrDefault();
            var includeInactive = request && role == 1;

            return Context.HeroSlides
                .Where(x => includeInactive || x.IsActive)
                .OrderBy(x => x.SortOrder).ThenBy(x => x.Id)
                .Select(x => new HeroSlideDto
                {
                    Id = x.Id,
                    Tagline = x.Tagline,
                    Title = x.Title,
                    Subtitle = x.Subtitle,
                    ButtonText = x.ButtonText,
                    ButtonLink = x.ButtonLink,
                    MediaType = x.MediaType,
                    MediaSrc = x.MediaSrc,
                    SortOrder = x.SortOrder,
                    IsActive = x.IsActive
                }).ToList();
        }
    }
}
