namespace EShopper.Domain
{
    public class PromotionGroupProduct : Entity
    {
        public int PromotionGroupId { get; set; }
        public int ProductId { get; set; }

        public decimal BasePrice { get; set; }

        public virtual PromotionGroup PromotionGroup { get; set; }
        public virtual Product Product { get; set; }
    }
}
