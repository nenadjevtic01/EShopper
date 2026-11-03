using EShopper.Application.UseCases.DTO;

namespace EShopper.Application.UseCases.Queries
{
    public interface IGetActivePromotionsQuery : IQuery<int, IEnumerable<ActivePromotionDto>>
    {
    }
}
