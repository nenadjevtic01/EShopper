using EShopper.Application.UseCases.DTO;
using EShopper.Application.UseCases.Queries;
using EShopper.DataAccess;

namespace EShopper.Implementation.UseCases.Queries
{
    public class GetBrandQueryEf : UseCaseEf, IGetBrandQuery
    {
        public int UseCaseId => 26;

        public string UseCaseName => "Get brands";

        public string UseCaseDescription => "Get brands with/without keyword.";

        public GetBrandQueryEf(EShopperDbContext context) : base(context)
        {
        }

        public PageResponse<BrandDto> Execute(PagedSearchDto request)
        {
            var query = Context.Brands.Where(x => request.IncludeInactive == true || x.IsActive);

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.BrandName.Contains(request.Keyword));
            }

            if (request.PerPage == null || request.PerPage < 1)
            {
                request.PerPage = 15;
            }

            if (request.Page == null || request.Page < 1)
            {
                request.Page = 1;
            }

            var toSkip = (request.Page.Value - 1) * request.PerPage.Value;

            var response = new PageResponse<BrandDto>();
            response.Count = query.Count();
            response.Data = query.Skip(toSkip).Take(request.PerPage.Value).Select(x => new BrandDto
            {
                Id = x.Id,
                Name = x.BrandName,
                Picture = new PictureDto
                {
                    Src = x.Src,
                    Alt = x.Alt
                },
                IsActive = x.IsActive
            }).ToList();
            response.CurrentPage = request.Page.Value;
            response.PerPage = request.PerPage.Value;

            return response;
        }
    }
}
