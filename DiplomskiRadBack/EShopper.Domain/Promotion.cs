namespace EShopper.Domain
{
    public class Promotion : Entity
    {
        public string Name { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidUntil { get; set; }

        public virtual ICollection<PromotionGroup> Groups { get; set; } = new List<PromotionGroup>();
    }
}
