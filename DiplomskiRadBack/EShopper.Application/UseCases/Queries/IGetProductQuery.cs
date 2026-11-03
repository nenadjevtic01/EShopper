using EShopper.Application.UseCases.DTO;

namespace EShopper.Application.UseCases.Queries
{
    public interface IGetProductQuery : IQuery<SearchProductDto, PageResponse<ProductDto>>
    {
    }
}
