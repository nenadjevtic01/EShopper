using FluentValidation;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Commands
{
    public class CreatePromotionCommandEf : UseCaseEf, ICreatePromotionCommand
    {
        private readonly IUser _user;

        public int UseCaseId => 46;

        public string UseCaseName => "Create promotion";

        public string UseCaseDescription => "Create promotion with discount groups.";

        public CreatePromotionCommandEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public void Execute(CreatePromotionDto request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User don't have permission to create promotions.");
            }

            var now = DateTime.UtcNow;
            var groups = PromotionRules.Validate(
                Context,
                request.Name,
                request.ValidFrom,
                request.ValidUntil,
                request.Groups,
                null,
                null,
                true,
                now);

            var promotion = new Promotion
            {
                Name = request.Name.Trim(),
                IsActive = true,
                ValidFrom = request.ValidFrom.Date,
                ValidUntil = request.ValidUntil.Date
            };

            Context.Promotions.Add(promotion);

            var items = new List<PromotionRules.Item>();

            foreach (var group in groups)
            {
                var promotionGroup = new PromotionGroup
                {
                    Promotion = promotion,
                    DiscountPercentage = group.DiscountPercentage
                };

                Context.PromotionGroups.Add(promotionGroup);

                foreach (var productId in group.ProductIds)
                {
                    var basePrice = PromotionRules.BasePrice(Context, productId, now);

                    Context.PromotionGroupProducts.Add(new PromotionGroupProduct
                    {
                        PromotionGroup = promotionGroup,
                        ProductId = productId,
                        BasePrice = basePrice
                    });

                    items.Add(new PromotionRules.Item { ProductId = productId, DiscountPercentage = group.DiscountPercentage, BasePrice = basePrice });
                }
            }

            PromotionRules.WritePrices(Context, promotion, items, now);

            Context.SaveChanges();
        }
    }
}
