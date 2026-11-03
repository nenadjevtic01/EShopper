using EShopper.Application.UseCases.DTO;

namespace EShopper.Application.UseCases.Queries
{
    public interface IGetBrandQuery : IQuery<PagedSearchDto, PageResponse<BrandDto>>
    {
    }
}
