namespace EShopper.Domain
{
    public class PromotionGroup : Entity
    {
        public int PromotionId { get; set; }
        public int DiscountPercentage { get; set; }

        public virtual Promotion Promotion { get; set; }
        public virtual ICollection<PromotionGroupProduct> Products { get; set; } = new List<PromotionGroupProduct>();
    }
}
