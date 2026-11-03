using EShopper.Application.UseCases.DTO;

namespace EShopper.Application.UseCases.Queries
{
    public interface IGetGenderQuery : IQuery<int, IEnumerable<GenderDto>>
    {
    }
}
