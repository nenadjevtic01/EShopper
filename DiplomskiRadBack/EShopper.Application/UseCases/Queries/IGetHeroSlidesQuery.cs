using EShopper.Application.UseCases.DTO;

namespace EShopper.Application.UseCases.Queries
{
    public interface IGetHeroSlidesQuery : IQuery<bool, IEnumerable<HeroSlideDto>>
    {
    }
}
