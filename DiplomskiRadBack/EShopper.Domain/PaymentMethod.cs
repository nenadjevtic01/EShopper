namespace EShopper.Domain
{
    public class PaymentMethod : Entity
    {
        public string Name { get; set; }

        public virtual ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();
    }
}
