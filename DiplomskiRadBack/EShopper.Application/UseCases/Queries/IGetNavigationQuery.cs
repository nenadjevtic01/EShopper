using EShopper.Application.UseCases.DTO;

namespace EShopper.Application.UseCases.Queries
{
    public interface IGetNavigationQuery : IQuery<int, IEnumerable<NavigationItemDto>>
    {
    }
}
