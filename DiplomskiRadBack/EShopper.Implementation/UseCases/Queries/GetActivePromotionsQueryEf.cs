using EShopper.Application.UseCases.DTO;
using EShopper.Application.UseCases.Queries;
using EShopper.DataAccess;

namespace EShopper.Implementation.UseCases.Queries
{
    public class GetActivePromotionsQueryEf : UseCaseEf, IGetActivePromotionsQuery
    {
        public int UseCaseId => 50;

        public string UseCaseName => "Get active promotions";

        public string UseCaseDescription => "Get active promotions for home page.";

        public GetActivePromotionsQueryEf(EShopperDbContext context) : base(context)
        {
        }

        public IEnumerable<ActivePromotionDto> Execute(int request)
        {
            var today = DateTime.UtcNow.Date;
            var promotions = Context.Promotions
                .Where(x => x.IsActive && x.ValidFrom <= today && x.ValidUntil >= today)
                .OrderBy(x => x.ValidFrom)
                .Select(x => new ActivePromotionDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Products = x.Groups.SelectMany(g => g.Products.Select(p => new
                    {
                        p.ProductId,
                        g.DiscountPercentage
                    })).Select(gp => new PromotionProductCardDto
                    {
                        Id = gp.ProductId,
                        ProductName = Context.Products.Where(pr => pr.Id == gp.ProductId).Select(pr => pr.ProductName).First(),
                        DiscountPercentage = gp.DiscountPercentage,
                        NewPrice = Context.Prices
                            .Where(pr => pr.ProductId == gp.ProductId && pr.ActiveFrom < DateTime.UtcNow)
                            .OrderByDescending(pr => pr.ActiveFrom)
                            .Select(pr => pr.NewPrice)
                            .First(),
                        OldPrice = Context.Prices
                            .Where(pr => pr.ProductId == gp.ProductId && pr.ActiveFrom < DateTime.UtcNow)
                            .OrderByDescending(pr => pr.ActiveFrom)
                            .Select(pr => pr.PromotionId != null ? pr.OldPrice : null)
                            .FirstOrDefault(),
                        InStock = Context.Products.Where(pr => pr.Id == gp.ProductId).Select(pr => pr.InStock).First(),
                        Picture = Context.Pictures
                            .Where(pic => pic.ProductId == gp.ProductId)
                            .Select(pic => new PictureDto { Src = pic.Src, Alt = pic.Alt })
                    }).ToList()
                })
                .ToList();

            foreach (var promotion in promotions)
            {
                promotion.Products = promotion.Products.GroupBy(p => p.Id).Select(g => g.First()).ToList();
            }

            return promotions;
        }
    }
}
