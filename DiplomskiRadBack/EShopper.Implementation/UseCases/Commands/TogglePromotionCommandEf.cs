using Microsoft.EntityFrameworkCore;
using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.Commands;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Commands
{
    public class TogglePromotionCommandEf : UseCaseEf, ITogglePromotionCommand
    {
        private readonly IUser _user;

        public int UseCaseId => 64;

        public string UseCaseName => "Toggle promotion";

        public string UseCaseDescription => "Toggle promotion active status.";

        public TogglePromotionCommandEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public void Execute(int request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User don't have permission to change promotions.");
            }

            var promotion = Context.Promotions.FirstOrDefault(x => x.Id == request);

            if (promotion == null)
            {
                throw new EntityNotExistException(nameof(Promotion), request);
            }

            var now = DateTime.UtcNow;

            if (!promotion.IsActive && promotion.ValidUntil.Date < now.Date)
            {
                throw PromotionRules.Fail("An expired promotion can not be activated again.");
            }

            var groupProducts = Context.PromotionGroupProducts
                .Where(x => x.PromotionGroup.PromotionId == promotion.Id)
                .Include(x => x.PromotionGroup)
                .ToList();

            if (promotion.IsActive)
            {
                var wasApplied = PromotionRules.IsRunning(promotion, now);
                PromotionRules.RemoveScheduledPrices(Context, promotion.Id, now);

                if (wasApplied)
                {
                    foreach (var groupProduct in groupProducts)
                    {
                        PromotionRules.RevertNow(
                            Context,
                            groupProduct.ProductId,
                            PromotionRules.BasePrice(Context, groupProduct.ProductId, now),
                            now);
                    }
                }

                promotion.IsActive = false;
            }
            else
            {
                promotion.IsActive = true;

                var items = new List<PromotionRules.Item>();

                foreach (var groupProduct in groupProducts)
                {
                    groupProduct.BasePrice = PromotionRules.BasePrice(Context, groupProduct.ProductId, now);
                    items.Add(new PromotionRules.Item
                    {
                        ProductId = groupProduct.ProductId,
                        DiscountPercentage = groupProduct.PromotionGroup.DiscountPercentage,
                        BasePrice = groupProduct.BasePrice
                    });
                }

                PromotionRules.WritePrices(Context, promotion, items, now);
            }

            Context.Promotions.Update(promotion);
            Context.SaveChanges();
        }
    }
}
