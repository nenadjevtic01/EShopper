using Microsoft.EntityFrameworkCore;
using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.DTO;
using EShopper.Application.UseCases.Queries;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Queries
{
    public class GetSingleProductQueryEf : UseCaseEf, IGetSingleProductQuery
    {
        public int UseCaseId => 7;

        public string UseCaseName => "Get single product";

        public string UseCaseDescription => "Detailed product";

        public GetSingleProductQueryEf(EShopperDbContext context) : base(context)
        {
        }

        public SingleProductDto Execute(int request)
        {
            var today = DateTime.UtcNow.Date;
            var product = Context.Products
                .Include(x => x.Gender)
                .Include(x => x.Category)
                .Include(x => x.Brand)
                .Include(x => x.Country)
                .Include(x => x.Prices)
                .Include(x => x.Pictures)
                .FirstOrDefault(x => x.Id == request && x.IsActive);

            if (product == null || !product.Prices.Any(x => x.ActiveFrom < DateTime.UtcNow))
            {
                throw new EntityNotExistException(nameof(Product), request);
            }

            var ratings = Context.ProductRatings
                .Include(x => x.User)
                .Where(x => x.ProductId == request && x.IsActive)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return new SingleProductDto
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Brand = product.BrandId,
                Gender = product.GenderId,
                Category = product.CategoryId,
                Sale = Context.PromotionGroupProducts.Any(pgp =>
                    pgp.ProductId == product.Id
                    && pgp.PromotionGroup.Promotion.IsActive
                    && pgp.PromotionGroup.Promotion.ValidFrom <= today
                    && pgp.PromotionGroup.Promotion.ValidUntil >= today),
                InStock = product.InStock,
                Material = product.Material,
                CountryOfOrigin = product.Country.Name,
                NewPrice = product.Prices
                    .OrderByDescending(x => x.ActiveFrom)
                    .Where(x => x.ActiveFrom < DateTime.UtcNow)
                    .Select(x => x.NewPrice)
                    .First(),
                OldPrice = product.Prices
                    .OrderByDescending(x => x.ActiveFrom)
                    .Where(x => x.ActiveFrom < DateTime.UtcNow)
                    .Select(x => x.PromotionId != null ? x.OldPrice : null)
                    .FirstOrDefault(),
                Pictures = product.Pictures.Select(x => new PictureDto
                {
                    Src = x.Src,
                    Alt = x.Alt
                }).ToList(),
                Sizes = product.ProductSizes.Select(x => new SizeDto
                {
                    Id = x.SizeId,
                    Name = x.Size.SizeName,
                    Quantity = x.Quantity
                }).ToList(),
                AverageRating = ratings.Count > 0 ? ratings.Average(x => x.Rating) : (decimal?)null,
                RatingsCount = ratings.Count,
                Ratings = ratings.Select(x => new ProductRatingDto
                {
                    Rating = x.Rating,
                    Comment = x.Comment,
                    UserName = x.User.FirstName + " " + x.User.LastName,
                    CreatedAt = x.CreatedAt
                }).ToList()
            };
        }
    }
}
