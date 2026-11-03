using EShopper.DataAccess;

namespace EShopper.Implementation.UseCases
{
    public static class PromotionExpiry
    {
        public static int DeactivateExpired(EShopperDbContext context, DateTime now)
        {
            var today = now.Date;
            var expired = context.Promotions.Where(x => x.IsActive && x.ValidUntil < today).ToList();

            foreach (var promotion in expired)
            {
                promotion.IsActive = false;
            }

            if (expired.Count > 0)
            {
                context.SaveChanges();
            }

            return expired.Count;
        }
    }
}
