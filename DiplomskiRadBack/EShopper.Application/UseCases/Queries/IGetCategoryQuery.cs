using EShopper.Application.UseCases.DTO;

namespace EShopper.Application.UseCases.Queries
{
    public interface IGetCategoryQuery : IQuery<PagedSearchDto, PageResponse<CategoryDto>>
    {
    }
}
