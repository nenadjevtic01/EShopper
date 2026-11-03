namespace EShopper.Domain
{
    public class Receipt : Entity
    {
        public int UserId { get; set; }
        public int PaymentMethodId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Total { get; set; }

        public string? BillingFirstName { get; set; }
        public string? BillingLastName { get; set; }
        public string? BillingEmail { get; set; }
        public string? BillingAddress { get; set; }
        public string? BillingCity { get; set; }
        public string? BillingCountry { get; set; }
        public string? BillingPostalCode { get; set; }

        public int? CouponId { get; set; }
        public decimal DiscountAmount { get; set; }

        public virtual User User { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }
        public virtual Coupon? Coupon { get; set; }
        public virtual ICollection<ReceiptItem> ReceiptItems { get; set; } = new List<ReceiptItem>();
    }
}
