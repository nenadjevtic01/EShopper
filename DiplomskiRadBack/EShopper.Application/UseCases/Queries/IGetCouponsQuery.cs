using EShopper.Application.UseCases.DTO;

namespace EShopper.Application.UseCases.Queries
{
    public interface IGetCouponsQuery : IQuery<PagedSearchDto, PageResponse<CouponDto>>
    {
    }
}
