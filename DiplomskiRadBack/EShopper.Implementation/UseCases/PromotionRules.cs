using FluentValidation;
using FluentValidation.Results;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases
{
    internal static class PromotionRules
    {
        public class Item
        {
            public int ProductId { get; set; }
            public int DiscountPercentage { get; set; }
            public decimal BasePrice { get; set; }
        }

        public static bool IsRunning(Promotion promotion, DateTime now)
        {
            return promotion.IsActive && promotion.ValidFrom.Date <= now.Date && now.Date <= promotion.ValidUntil.Date;
        }

        public static decimal BasePrice(EShopperDbContext context, int productId, DateTime now)
        {
            return context.Prices
                .Where(x => x.ProductId == productId && x.PromotionId == null && x.ActiveFrom < now)
                .OrderByDescending(x => x.ActiveFrom)
                .ThenByDescending(x => x.Id)
                .Select(x => x.NewPrice)
                .First();
        }

        public static List<CreatePromotionGroupDto> Validate(EShopperDbContext context, string name, DateTime validFrom, DateTime validUntil,
            IEnumerable<CreatePromotionGroupDto> requestGroups, int? excludePromotionId, ICollection<int> alreadyInPromotion, bool datesChanged, DateTime now)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw Fail("Promotion name is required.");
            }

            if (name.Trim().Length > 100)
            {
                throw Fail("Promotion name must be 100 characters or fewer.");
            }

            if (validFrom.Date > validUntil.Date)
            {
                throw Fail("Valid until date can't be before the valid from date.");
            }

            if (datesChanged && validUntil.Date < now.Date)
            {
                throw Fail("Valid until date can't be in the past.");
            }

            var from = validFrom.Date;
            var until = validUntil.Date;

            if (context.Promotions.Any(x => x.Id != excludePromotionId && x.ValidFrom.Date <= until && from <= x.ValidUntil.Date))
            {
                throw new Exception("This promotion's dates overlap with another promotion's dates.");
            }

            var groups = requestGroups?.ToList();

            if (groups == null || groups.Count == 0)
            {
                throw Fail("A promotion needs at least one discount group.");
            }

            var allProductIds = groups.SelectMany(g => g.ProductIds ?? Enumerable.Empty<int>()).ToList();

            if (allProductIds.Count == 0)
            {
                throw Fail("Each discount group needs at least one product.");
            }

            if (allProductIds.Count != allProductIds.Distinct().Count())
            {
                throw Fail("A product can only be in one discount group per promotion.");
            }

            foreach (var group in groups)
            {
                if (group.DiscountPercentage < 1 || group.DiscountPercentage > 100)
                {
                    throw Fail("Discount percentage must be between 1 and 100.");
                }

                if (group.ProductIds == null || !group.ProductIds.Any())
                {
                    throw Fail("Each discount group needs at least one product.");
                }
            }

            var products = context.Products.Where(x => allProductIds.Contains(x.Id))
                .Select(x => new { x.Id, x.ProductName, x.IsActive, x.InStock }).ToList();
            if (products.Count != allProductIds.Distinct().Count())
            {
                throw Fail("One or more selected products do not exist.");
            }

            var notAllowed = products.FirstOrDefault(x =>
                (!x.IsActive || !x.InStock)
                && !(alreadyInPromotion != null && alreadyInPromotion.Contains(x.Id)));

            if (notAllowed != null)
            {
                throw Fail($"Product '{notAllowed.ProductName}' is not active or is out of stock.");
            }

            return groups;
        }

        public static ValidationException Fail(string message)
        {
            return new ValidationException(new[] { new ValidationFailure("Promotion", message) });
        }

        public static void WritePrices(EShopperDbContext context, Promotion promotion, IEnumerable<Item> items, DateTime now)
        {
            var start = promotion.ValidFrom.Date;
            var end = promotion.ValidUntil.Date.AddDays(1).AddSeconds(-1);

            if (end <= now)
            {
                return;
            }

            foreach (var item in items)
            {
                context.Prices.Add(new Price
                {
                    ProductId = item.ProductId,
                    NewPrice = Math.Round(item.BasePrice * (100 - item.DiscountPercentage) / 100m, 2),
                    OldPrice = item.BasePrice,
                    ActiveFrom = start > now ? start : now,
                    Promotion = promotion
                });

                context.Prices.Add(new Price
                {
                    ProductId = item.ProductId,
                    NewPrice = item.BasePrice,
                    OldPrice = null,
                    ActiveFrom = end,
                    Promotion = promotion
                });
            }
        }

        public static void RemoveScheduledPrices(EShopperDbContext context, int promotionId, DateTime now)
        {
            var scheduled = context.Prices.Where(x => x.PromotionId == promotionId && x.ActiveFrom > now).ToList();
            context.Prices.RemoveRange(scheduled);
        }

        public static void RevertNow(EShopperDbContext context, int productId, decimal basePrice, DateTime now)
        {
            context.Prices.Add(new Price
            {
                ProductId = productId,
                NewPrice = basePrice,
                OldPrice = null,
                ActiveFrom = now
            });
        }

        public static void Reprice(EShopperDbContext context, int productId, decimal basePrice, DateTime now)
        {
            var today = now.Date;
            var groupProducts = context.PromotionGroupProducts
                .Where(x => x.ProductId == productId
                    && x.PromotionGroup.Promotion.IsActive
                    && x.PromotionGroup.Promotion.ValidUntil >= today)
                .ToList();

            foreach (var groupProduct in groupProducts)
            {
                var promotion = groupProduct.PromotionGroup.Promotion;
                var scheduled = context.Prices
                    .Where(x => x.PromotionId == promotion.Id && x.ProductId == productId && x.ActiveFrom > now)
                    .ToList();

                context.Prices.RemoveRange(scheduled);
                groupProduct.BasePrice = basePrice;

                WritePrices(context, promotion, new[]
                {
                    new Item
                    {
                        ProductId = productId,
                        DiscountPercentage = groupProduct.PromotionGroup.DiscountPercentage,
                        BasePrice = basePrice
                    }
                }, now);
            }
        }
    }
}
