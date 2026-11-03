namespace EShopper.Application.UseCases.DTO
{
    public class ConfirmOrderDto
    {
        public int UserId { get; set; }
        public int PaymentMethodId { get; set; }

        public string BillingFirstName { get; set; }
        public string BillingLastName { get; set; }
        public string BillingEmail { get; set; }
        public string BillingAddress { get; set; }
        public string BillingCity { get; set; }
        public string BillingCountry { get; set; }
        public string BillingPostalCode { get; set; }

        public string? CouponCode { get; set; }
    }
}
