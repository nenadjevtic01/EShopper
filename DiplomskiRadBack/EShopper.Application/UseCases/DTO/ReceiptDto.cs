namespace EShopper.Application.UseCases.DTO
{
    public class ReceiptDto
    {
        public int ReceiptId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Total { get; set; }
        public bool IsActive { get; set; }

        public DateTime OrderedAt { get; set; }

        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }

        public int PaymentMethodId { get; set; }
        public string PaymentMethodName { get; set; }

        public string BillingFirstName { get; set; }
        public string BillingLastName { get; set; }
        public string BillingEmail { get; set; }
        public string BillingAddress { get; set; }
        public string BillingCity { get; set; }
        public string BillingCountry { get; set; }
        public string BillingPostalCode { get; set; }

        public string CouponCode { get; set; }
        public decimal DiscountAmount { get; set; }

        public IEnumerable<ReceiptItemDto> Items { get; set; }
    }
}
