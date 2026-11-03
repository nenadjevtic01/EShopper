using Microsoft.EntityFrameworkCore;
using EShopper.Application.UseCases.DTO;
using EShopper.Application.UseCases.Queries;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Queries
{
    public class GetProductsQueryEf : UseCaseEf, IGetProductQuery
    {
        public int UseCaseId => 5;

        public string UseCaseName => "Get products";

        public string UseCaseDescription => "Search and filter products.";

        public GetProductsQueryEf(EShopperDbContext context) : base(context)
        {
        }

        public PageResponse<ProductDto> Execute(SearchProductDto request)
        {
            var today = DateTime.UtcNow.Date;
            var query = Context.Products.Include(x => x.Category).Include(x => x.Prices)
                .Where(x => request.IncludeInactive == true || x.IsActive)
                .Where(x => x.Prices.Any(p => p.ActiveFrom < DateTime.UtcNow))
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.ProductName.Contains(request.Keyword));
            }

            if (request.CategoryId != null && Context.Categories.Any(x => x.Id == request.CategoryId))
            {
                query = query.Where(x => x.CategoryId == request.CategoryId);
            }

            if (request.BrandId != null && Context.Brands.Any(x => x.Id == request.BrandId))
            {
                query = query.Where(x => x.BrandId == request.BrandId);
            }

            if (request.MinimumPrice != null)
            {
                query = query.Where(x =>
                    x.Prices
                        .OrderByDescending(x => x.ActiveFrom)
                        .Where(x => x.ActiveFrom < DateTime.UtcNow)
                        .Select(x => x.NewPrice)
                        .First() > request.MinimumPrice);
            }

            if (request.MaximumPrice != null)
            {
                query = query.Where(x =>
                    x.Prices
                        .OrderByDescending(x => x.ActiveFrom)
                        .Where(x => x.ActiveFrom < DateTime.UtcNow)
                        .Select(x => x.NewPrice)
                        .First() < request.MaximumPrice);
            }

            if (request.CategoryIds != null && request.CategoryIds.Count > 0)
            {
                query = query.Where(x => request.CategoryIds.Contains(x.CategoryId));
            }

            if (request.BrandIds != null && request.BrandIds.Count > 0)
            {
                query = query.Where(x => request.BrandIds.Contains(x.BrandId));
            }

            if (request.GenderIds != null && request.GenderIds.Count > 0)
            {
                query = query.Where(x => request.GenderIds.Contains(x.GenderId));
            }

            if (request.SizeIds != null && request.SizeIds.Count > 0)
            {
                query = query.Where(x => x.ProductSizes.Any(ps => request.SizeIds.Contains(ps.SizeId)));
            }

            if (request.OnlyInStock == true)
            {
                query = query.Where(x => x.InStock);
            }

            if (request.PerPage == null || request.PerPage < 1)
            {
                request.PerPage = 15;
            }

            if (request.PerPage > 500)
            {
                request.PerPage = 500;
            }

            if (request.Page == null || request.Page < 1)
            {
                request.Page = 1;
            }

            var descending = string.Equals(request.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
            IOrderedQueryable<Product> ordered;

            var sortKey = (request.SortBy ?? "").ToLower();

            switch (sortKey)
            {
                case "name":
                    ordered = descending ? query.OrderByDescending(x => x.ProductName) : query.OrderBy(x => x.ProductName);
                    break;

                case "price":
                    ordered = descending
                        ? query.OrderByDescending(x =>
                            x.Prices
                                .OrderByDescending(p => p.ActiveFrom)
                                .Where(p => p.ActiveFrom < DateTime.UtcNow)
                                .Select(p => p.NewPrice)
                                .First())
                        : query.OrderBy(x =>
                            x.Prices
                                .OrderByDescending(p => p.ActiveFrom)
                                .Where(p => p.ActiveFrom < DateTime.UtcNow)
                                .Select(p => p.NewPrice)
                                .First());
                    break;

                case "sale":
                    ordered = descending
                        ? query.OrderByDescending(x =>
                            Context.PromotionGroupProducts.Any(pgp =>
                                pgp.ProductId == x.Id
                                && pgp.PromotionGroup.Promotion.IsActive
                                && pgp.PromotionGroup.Promotion.ValidFrom <= today
                                && pgp.PromotionGroup.Promotion.ValidUntil >= today))
                        : query.OrderBy(x =>
                            Context.PromotionGroupProducts.Any(pgp =>
                                pgp.ProductId == x.Id
                                && pgp.PromotionGroup.Promotion.IsActive
                                && pgp.PromotionGroup.Promotion.ValidFrom <= today
                                && pgp.PromotionGroup.Promotion.ValidUntil >= today));
                    break;

                case "instock":
                    ordered = descending ? query.OrderByDescending(x => x.InStock) : query.OrderBy(x => x.InStock);
                    break;

                case "updated":
                    ordered = descending ? query.OrderByDescending(x => x.UpdatedAt) : query.OrderBy(x => x.UpdatedAt);
                    break;

                case "random":
                    ordered = query.OrderBy(x => Guid.NewGuid());
                    break;

                default:
                    ordered = descending ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id);
                    break;
            }

            if (sortKey != "random" && sortKey != "" && sortKey != "id")
            {
                ordered = ordered.ThenBy(x => x.Id);
            }

            var toSkip = (request.Page.Value - 1) * request.PerPage.Value;

            var response = new PageResponse<ProductDto>();
            response.Count = query.Count();
            response.Data = ordered.Skip(toSkip).Take(request.PerPage.Value).Select(x => new ProductDto
            {
                Id = x.Id,
                ProductName = x.ProductName,
                CategoryId = x.CategoryId,
                BrandId = x.BrandId,
                GenderId = x.GenderId,
                NewPrice = x.Prices
                    .OrderByDescending(x => x.ActiveFrom)
                    .Where(x => x.ActiveFrom < DateTime.UtcNow)
                    .Select(x => x.NewPrice)
                    .First(),
                OldPrice = x.Prices
                    .OrderByDescending(x => x.ActiveFrom)
                    .Where(x => x.ActiveFrom < DateTime.UtcNow)
                    .Select(x => x.PromotionId != null ? x.OldPrice : null)
                    .FirstOrDefault(),
                InStock = x.InStock,
                Sale = Context.PromotionGroupProducts.Any(pgp =>
                    pgp.ProductId == x.Id
                    && pgp.PromotionGroup.Promotion.IsActive
                    && pgp.PromotionGroup.Promotion.ValidFrom <= today
                    && pgp.PromotionGroup.Promotion.ValidUntil >= today),
                IsActive = x.IsActive,
                Material = x.Material,
                CountryId = x.CountryId,
                CountryOfOrigin = x.Country.Name,
                UpdatedAt = x.UpdatedAt,
                Picture = x.Pictures.Select(x => new PictureDto
                {
                    Src = x.Src,
                    Alt = x.Alt
                }),
                Sizes = x.ProductSizes.Select(x => x.Size.SizeName).ToArray(),
                SizeStocks = x.ProductSizes.Select(x => new SizeDto
                {
                    Id = x.SizeId,
                    Name = x.Size.SizeName,
                    Quantity = x.Quantity
                }).ToList(),
            }).ToList();

            response.CurrentPage = request.Page.Value;
            response.PerPage = request.PerPage.Value;

            return response;
        }
    }
}
