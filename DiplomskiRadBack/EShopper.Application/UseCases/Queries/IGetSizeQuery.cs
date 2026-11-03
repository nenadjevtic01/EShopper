using EShopper.Application.UseCases.DTO;

namespace EShopper.Application.UseCases.Queries
{
    public interface IGetSizeQuery : IQuery<int, IEnumerable<SizeDto>>
    {
    }
}
