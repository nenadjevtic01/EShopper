using EShopper.Application.UseCases.DTO;
using EShopper.Application.UseCases.Queries;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Queries
{
    public class GetPromotionsQueryEf : UseCaseEf, IGetPromotionsQuery
    {
        private readonly IUser _user;

        public int UseCaseId => 49;

        public string UseCaseName => "Get promotions";

        public string UseCaseDescription => "Get promotions with groups and products.";

        public GetPromotionsQueryEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public PageResponse<PromotionDto> Execute(PagedSearchDto request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User don't have permission to view promotions.");
            }

            var query = Context.Promotions.Where(x => request.IncludeInactive == true || x.IsActive).AsQueryable();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.Name.Contains(request.Keyword));
            }

            query = query.OrderByDescending(x => x.ValidFrom);

            if (request.PerPage == null || request.PerPage < 1)
            {
                request.PerPage = 15;
            }

            if (request.Page == null || request.Page < 1)
            {
                request.Page = 1;
            }

            var toSkip = (request.Page.Value - 1) * request.PerPage.Value;

            var response = new PageResponse<PromotionDto>();
            response.Count = query.Count();
            response.Data = query.Skip(toSkip).Take(request.PerPage.Value).Select(x => new PromotionDto
            {
                Id = x.Id,
                Name = x.Name,
                IsActive = x.IsActive,
                ValidFrom = x.ValidFrom,
                ValidUntil = x.ValidUntil,
                CreatedAt = x.CreatedAt,
                ProductCount = x.Groups.SelectMany(g => g.Products).Count(),
                Groups = x.Groups.Select(g => new PromotionGroupDto
                {
                    DiscountPercentage = g.DiscountPercentage,
                    Products = g.Products.Select(p => new PromotionGroupProductDto
                    {
                        ProductId = p.ProductId,
                        ProductName = p.Product.ProductName,
                        Picture = p.Product.Pictures.Select(pic => new PictureDto { Src = pic.Src, Alt = pic.Alt }).FirstOrDefault()
                    }).ToList()
                }).ToList()
            }).ToList();
            response.CurrentPage = request.Page.Value;
            response.PerPage = request.PerPage.Value;

            return response;
        }
    }
}
