using Microsoft.EntityFrameworkCore;
using EShopper.Application.UseCases.DTO;
using EShopper.Application.UseCases.Queries;
using EShopper.DataAccess;
using EShopper.Domain;
using System.Globalization;

namespace EShopper.Implementation.UseCases.Queries
{
    public class GetAdminStatsQueryEf : UseCaseEf, IGetAdminStatsQuery
    {
        private readonly IUser _user;

        public int UseCaseId => 32;

        public string UseCaseName => "Get admin stats";

        public string UseCaseDescription => "Orders/revenue/category/brand breakdown.";

        public GetAdminStatsQueryEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public AdminStatsDto Execute(int request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User don't have permission to view admin stats.");
            }

            var receipts = Context.Receipts.Include(x => x.PaymentMethod).Where(x => x.IsActive).ToList();

            var monthlyStats = receipts
                .GroupBy(x => new { x.CreatedAt.Year, x.CreatedAt.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new MonthlyStatDto
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy", CultureInfo.InvariantCulture),
                    OrderCount = g.Count(),
                    Revenue = g.Sum(x => x.Total)
                })
                .ToList();

            var items = Context.ReceiptsItems
                .Where(x => x.Receipt.IsActive)
                .Select(x => new
                {
                    x.Quantity,
                    CategoryName = x.Product.Category.CategoryName,
                    BrandName = x.Product.Brand.BrandName
                })
                .ToList();

            var totalQuantity = items.Sum(x => x.Quantity);

            var categoryShares = items
                .GroupBy(x => x.CategoryName)
                .Select(g => new ShareDto
                {
                    Name = g.Key,
                    QuantitySold = g.Sum(x => x.Quantity),
                    Percentage = totalQuantity == 0 ? 0 : Math.Round(g.Sum(x => x.Quantity) * 100m / totalQuantity, 1)
                })
                .OrderByDescending(x => x.QuantitySold)
                .ToList();

            var brandShares = items
                .GroupBy(x => x.BrandName)
                .Select(g => new ShareDto
                {
                    Name = g.Key,
                    QuantitySold = g.Sum(x => x.Quantity),
                    Percentage = totalQuantity == 0 ? 0 : Math.Round(g.Sum(x => x.Quantity) * 100m / totalQuantity, 1)
                })
                .OrderByDescending(x => x.QuantitySold)
                .ToList();

            var paymentMethodShares = receipts
                .GroupBy(x => x.PaymentMethod.Name)
                .Select(g => new PaymentMethodShareDto
                {
                    Name = g.Key,
                    OrderCount = g.Count(),
                    Percentage = receipts.Count == 0 ? 0 : Math.Round(g.Count() * 100m / receipts.Count, 1)
                })
                .OrderByDescending(x => x.OrderCount)
                .ToList();

            return new AdminStatsDto
            {
                TotalOrders = receipts.Count,
                TotalRevenue = receipts.Sum(x => x.Total),
                AverageOrderValue = receipts.Count == 0 ? 0 : Math.Round(receipts.Sum(x => x.Total) / receipts.Count, 2),
                MonthlyStats = monthlyStats,
                CategoryShares = categoryShares,
                BrandShares = brandShares,
                PaymentMethodShares = paymentMethodShares
            };
        }
    }
}
