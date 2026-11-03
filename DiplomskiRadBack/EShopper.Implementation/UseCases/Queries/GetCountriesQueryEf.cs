using EShopper.Application.UseCases.DTO;
using EShopper.Application.UseCases.Queries;
using EShopper.DataAccess;

namespace EShopper.Implementation.UseCases.Queries
{
    public class GetCountriesQueryEf : UseCaseEf, IGetCountriesQuery
    {
        public int UseCaseId => 51;

        public string UseCaseName => "Get countries";

        public string UseCaseDescription => "Get countries for dropdown.";

        public GetCountriesQueryEf(EShopperDbContext context) : base(context)
        {
        }

        public IEnumerable<CountryDto> Execute(int request)
        {
            return Context.Countries.Where(x => x.IsActive).OrderBy(x => x.Name).Select(x => new CountryDto
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();
        }
    }
}
