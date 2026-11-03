namespace EShopper.Domain
{
    public class ReceiptItem : Entity
    {
        public int ProductId { get; set; }
        public int PriceId { get; set; }
        public int SizeId { get; set; }
        public int Quantity { get; set; }
        public int ReceiptId { get; set; }

        public virtual Price Price { get; set; }
        public virtual Product Product { get; set; }
        public virtual Receipt Receipt { get; set; }
        public virtual Size Size { get; set; }
    }
}
