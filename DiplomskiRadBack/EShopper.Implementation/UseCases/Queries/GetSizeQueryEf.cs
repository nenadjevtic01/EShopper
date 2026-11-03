using EShopper.Application.UseCases.DTO;
using EShopper.Application.UseCases.Queries;
using EShopper.DataAccess;

namespace EShopper.Implementation.UseCases.Queries
{
    public class GetSizeQueryEf : UseCaseEf, IGetSizeQuery
    {
        public int UseCaseId => 56;

        public string UseCaseName => "Get sizes";

        public string UseCaseDescription => "Get sizes for shop filter.";

        public GetSizeQueryEf(EShopperDbContext context) : base(context)
        {
        }

        public IEnumerable<SizeDto> Execute(int request)
        {
            return Context.Sizes.Where(x => x.IsActive).OrderBy(x => x.Id).Select(x => new SizeDto
            {
                Id = x.Id,
                Name = x.SizeName
            }).ToList();
        }
    }
}
