namespace EShopper.Domain
{
    public class ProductRating : Entity
    {
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public decimal Rating { get; set; }
        public string Comment { get; set; }

        public virtual Product Product { get; set; }
        public virtual User User { get; set; }
    }
}
