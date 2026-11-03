using EShopper.Application.UseCases.DTO;
using EShopper.Application.UseCases.Queries;
using EShopper.DataAccess;

namespace EShopper.Implementation.UseCases.Queries
{
    public class GetCategoryQueryEf : UseCaseEf, IGetCategoryQuery
    {
        public int UseCaseId => 1;

        public string UseCaseName => "Get categories";

        public string UseCaseDescription => "Get categories with/without keyword.";

        public GetCategoryQueryEf(EShopperDbContext context) : base(context)
        {

        }

        public PageResponse<CategoryDto> Execute(PagedSearchDto request)
        {
            var query = Context.Categories.Where(x => request.IncludeInactive == true || x.IsActive);

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.CategoryName.Contains(request.Keyword));
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

            var response = new PageResponse<CategoryDto>();
            response.Count = query.Count();
            response.Data = query.Skip(toSkip).Take(request.PerPage.Value).Select(x => new CategoryDto
            {
                Id = x.Id,
                Name = x.CategoryName,
                Products = x.Products.Select(y => new ProductDto
                {
                    Id = y.Id,
                    ProductName = y.ProductName,
                    CategoryId = y.CategoryId,
                    BrandId = y.BrandId,
                    GenderId = y.GenderId,
                    NewPrice = y.Prices
                        .OrderByDescending(x => x.ActiveFrom)
                        .Where(x => x.ActiveFrom < DateTime.UtcNow)
                        .Select(x => x.NewPrice)
                        .First(),
                    OldPrice = y.Prices
                        .OrderByDescending(x => x.ActiveFrom)
                        .Where(x => x.ActiveFrom < DateTime.UtcNow)
                        .Select(x => x.PromotionId != null ? x.OldPrice : null)
                        .First(),
                    Picture = y.Pictures.Select(x => new PictureDto
                    {
                        Src = x.Src,
                        Alt = x.Alt,
                    }),
                    Sizes = y.ProductSizes.Select(x => x.Size.SizeName).ToArray(),
                }).ToList(),
                Picture = new PictureDto
                {
                    Src = x.Src,
                    Alt = x.Alt,
                },
                IsActive = x.IsActive
            }); ;
            response.CurrentPage = request.Page.Value;
            response.PerPage = request.PerPage.Value;

            return response;
        }
    }
}
