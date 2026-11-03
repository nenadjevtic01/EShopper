using EShopper.Application.UseCases.DTO;

namespace EShopper.Application.UseCases.Queries
{
    public interface IGetPromotionsQuery : IQuery<PagedSearchDto, PageResponse<PromotionDto>>
    {
    }
}
