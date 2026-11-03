using EShopper.Application.UseCases.DTO;

namespace EShopper.Application.UseCases.Queries
{
    public interface IGetSingleUserQuery : IQuery<int, UserDtoResponse>
    {
    }
}
