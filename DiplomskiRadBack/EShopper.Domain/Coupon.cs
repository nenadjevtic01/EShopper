namespace EShopper.Domain
{
    public class Coupon : Entity
    {
        public string Code { get; set; }
        public int DiscountPercentage { get; set; }
        public DateTime ValidUntil { get; set; }

        public virtual ICollection<CouponUsage> Usages { get; set; } = new List<CouponUsage>();
    }
}
