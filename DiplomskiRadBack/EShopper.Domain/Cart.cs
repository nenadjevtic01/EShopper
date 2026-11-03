namespace EShopper.Domain
{
    public class Cart : Entity
    {
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
