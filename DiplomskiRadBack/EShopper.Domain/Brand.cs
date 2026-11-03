namespace EShopper.Domain
{
    public class Brand : Entity
    {
        public string BrandName { get; set; }

        public string Src { get; set; }
        public string Alt { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
