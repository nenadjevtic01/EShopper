using EShopper.Application.UseCases.DTO;
using EShopper.Application.UseCases.Queries;
using EShopper.DataAccess;

namespace EShopper.Implementation.UseCases.Queries
{
    public class GetGenderQueryEf : UseCaseEf, IGetGenderQuery
    {
        public int UseCaseId => 55;

        public string UseCaseName => "Get genders";

        public string UseCaseDescription => "Get genders for shop filter.";

        public GetGenderQueryEf(EShopperDbContext context) : base(context)
        {
        }

        public IEnumerable<GenderDto> Execute(int request)
        {
            return Context.Genders.Where(x => x.IsActive).OrderBy(x => x.Id).Select(x => new GenderDto
            {
                Id = x.Id,
                Name = x.GenderName
            }).ToList();
        }
    }
}
