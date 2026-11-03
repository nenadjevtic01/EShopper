using FluentValidation;
using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Commands
{
    public class UpdatePromotionCommandEf : UseCaseEf, IUpdatePromotionCommand
    {
        private readonly IUser _user;

        public int UseCaseId => 47;

        public string UseCaseName => "Update promotion";

        public string UseCaseDescription => "Update promotion name, dates and groups.";

        public UpdatePromotionCommandEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public void Execute(UpdatePromotionDto request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User don't have permission to update promotions.");
            }

            var promotion = Context.Promotions.FirstOrDefault(x => x.Id == request.Id);

            if (promotion == null)
            {
                throw new EntityNotExistException(nameof(Promotion), request.Id);
            }

            var now = DateTime.UtcNow;

            var oldGroups = Context.PromotionGroups.Where(x => x.PromotionId == promotion.Id).ToList();
            var oldGroupIds = oldGroups.Select(x => x.Id).ToList();
            var oldGroupProducts = Context.PromotionGroupProducts.Where(x => oldGroupIds.Contains(x.PromotionGroupId)).ToList();
            var oldProductIds = oldGroupProducts.Select(x => x.ProductId).Distinct().ToList();

            var datesChanged = promotion.ValidFrom.Date != request.ValidFrom.Date || promotion.ValidUntil.Date != request.ValidUntil.Date;
            var groups = PromotionRules.Validate(
                Context,
                request.Name,
                request.ValidFrom,
                request.ValidUntil,
                request.Groups,
                promotion.Id,
                oldProductIds,
                datesChanged,
                now);

            var wasApplied = PromotionRules.IsRunning(promotion, now);

            PromotionRules.RemoveScheduledPrices(Context, promotion.Id, now);

            Context.PromotionGroupProducts.RemoveRange(oldGroupProducts);
            Context.PromotionGroups.RemoveRange(oldGroups);

            promotion.Name = request.Name.Trim();
            promotion.ValidFrom = request.ValidFrom.Date;
            promotion.ValidUntil = request.ValidUntil.Date;

            var newProductIds = groups.SelectMany(g => g.ProductIds).ToList();
            var runsNow = PromotionRules.IsRunning(promotion, now);

            if (wasApplied)
            {
                foreach (var productId in oldProductIds)
                {
                    if (!(runsNow && newProductIds.Contains(productId)))
                    {
                        PromotionRules.RevertNow(Context, productId, PromotionRules.BasePrice(Context, productId, now), now);
                    }
                }
            }

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

            if (promotion.IsActive)
            {
                PromotionRules.WritePrices(Context, promotion, items, now);
            }

            Context.SaveChanges();
        }
    }
}
