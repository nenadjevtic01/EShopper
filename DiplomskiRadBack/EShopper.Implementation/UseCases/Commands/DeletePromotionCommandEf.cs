using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.Commands;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Commands
{
    public class DeletePromotionCommandEf : UseCaseEf, IDeletePromotionCommand
    {
        private readonly IUser _user;

        public int UseCaseId => 48;

        public string UseCaseName => "Delete promotion";

        public string UseCaseDescription => "Delete promotion and its scheduled prices.";

        public DeletePromotionCommandEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public void Execute(int request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User don't have permission to delete promotions.");
            }

            var promotion = Context.Promotions.FirstOrDefault(x => x.Id == request);

            if (promotion == null)
            {
                throw new EntityNotExistException(nameof(Promotion), request);
            }

            var now = DateTime.UtcNow;

            var groups = Context.PromotionGroups.Where(x => x.PromotionId == promotion.Id).ToList();
            var groupIds = groups.Select(x => x.Id).ToList();
            var groupProducts = Context.PromotionGroupProducts.Where(x => groupIds.Contains(x.PromotionGroupId)).ToList();

            if (PromotionRules.IsRunning(promotion, now))
            {
                foreach (var productId in groupProducts.Select(x => x.ProductId).Distinct())
                {
                    PromotionRules.RevertNow(Context, productId, PromotionRules.BasePrice(Context, productId, now), now);
                }
            }

            PromotionRules.RemoveScheduledPrices(Context, promotion.Id, now);
            var history = Context.Prices.Where(x => x.PromotionId == promotion.Id && x.ActiveFrom <= now).ToList();

            foreach (var price in history)
            {
                price.PromotionId = null;
            }

            Context.PromotionGroupProducts.RemoveRange(groupProducts);
            Context.PromotionGroups.RemoveRange(groups);
            Context.Promotions.Remove(promotion);
            Context.SaveChanges();
        }
    }
}
