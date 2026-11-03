namespace EShopper.Domain
{
    public class CartItem : Entity
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }

        public virtual Cart Cart { get; set; }
        public virtual Product Product { get; set; }

        public virtual Size Size { get; set; }
    }
}
