using EShopper.Application.UseCases.DTO;

namespace EShopper.Application.UseCases.Queries
{
    public interface IGetUserQuery : IQuery<PagedSearchDto, PageResponse<UserDtoResponse>>
    {
    }
}
