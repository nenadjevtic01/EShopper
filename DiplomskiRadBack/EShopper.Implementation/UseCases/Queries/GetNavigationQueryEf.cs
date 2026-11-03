using EShopper.Application.UseCases.DTO;
using EShopper.Application.UseCases.Queries;
using EShopper.DataAccess;

namespace EShopper.Implementation.UseCases.Queries
{
    public class GetNavigationQueryEf : UseCaseEf, IGetNavigationQuery
    {
        public int UseCaseId => 57;

        public string UseCaseName => "Get navigation";

        public string UseCaseDescription => "Get navigation links.";

        public GetNavigationQueryEf(EShopperDbContext context) : base(context)
        {
        }

        public IEnumerable<NavigationItemDto> Execute(int request)
        {
            return Context.NavigationItems
                .Where(x => x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.Id)
                .Select(x => new NavigationItemDto
            {
                Id = x.Id,
                Name = x.Name,
                Path = x.Path,
                DisplayOrder = x.DisplayOrder
            }).ToList();
        }
    }
}
