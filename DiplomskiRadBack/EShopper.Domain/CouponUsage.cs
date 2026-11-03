namespace EShopper.Domain
{
    public class CouponUsage : Entity
    {
        public int CouponId { get; set; }
        public int UserId { get; set; }
        public int ReceiptId { get; set; }

        public virtual Coupon Coupon { get; set; }
        public virtual User User { get; set; }
        public virtual Receipt Receipt { get; set; }
    }
}
