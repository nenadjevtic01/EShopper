namespace EShopper.Domain
{
    public class Size : Entity
    {
        public string SizeName { get; set; }

        public virtual ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public virtual ICollection<ReceiptItem> ReceiptItems { get; set; } = new List<ReceiptItem>();
    }
}
