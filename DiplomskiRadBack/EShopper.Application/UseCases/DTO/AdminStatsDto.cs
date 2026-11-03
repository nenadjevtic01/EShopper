namespace EShopper.Application.UseCases.DTO
{
    public class MonthlyStatDto
    {
        public string Month { get; set; }
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
    }

    public class ShareDto
    {
        public string Name { get; set; }
        public int QuantitySold { get; set; }
        public decimal Percentage { get; set; }
    }

    public class PaymentMethodShareDto
    {
        public string Name { get; set; }
        public int OrderCount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class AdminStatsDto
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public IEnumerable<MonthlyStatDto> MonthlyStats { get; set; }
        public IEnumerable<ShareDto> CategoryShares { get; set; }
        public IEnumerable<ShareDto> BrandShares { get; set; }
        public IEnumerable<PaymentMethodShareDto> PaymentMethodShares { get; set; }
        public decimal AverageOrderValue { get; set; }
    }
}
