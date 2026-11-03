using EShopper.Application.UseCases.DTO;

namespace EShopper.Application.UseCases.Queries
{
    public interface IGetCountriesQuery : IQuery<int, IEnumerable<CountryDto>>
    {
    }
}
