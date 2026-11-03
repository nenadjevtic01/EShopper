namespace EShopper.Application.UseCases.DTO
{
    public class CouponDto : BaseDto
    {
        public string Code { get; set; }
        public int DiscountPercentage { get; set; }
        public DateTime ValidUntil { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TimesUsed { get; set; }
        public IEnumerable<CouponUsageDto> Usages { get; set; }
    }

    public class CouponUsageDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public int ReceiptId { get; set; }
        public DateTime UsedAt { get; set; }
    }

    public class CreateCouponDto
    {
        public string Code { get; set; }
        public int DiscountPercentage { get; set; }
        public DateTime ValidUntil { get; set; }
    }

    public class UpdateCouponDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int DiscountPercentage { get; set; }
        public DateTime ValidUntil { get; set; }
    }

    public class ValidateCouponDto
    {
        public string Code { get; set; }
    }

    public class CouponValidationResultDto
    {
        public bool Valid { get; set; }
        public string Message { get; set; }
        public int DiscountPercentage { get; set; }
    }
}
