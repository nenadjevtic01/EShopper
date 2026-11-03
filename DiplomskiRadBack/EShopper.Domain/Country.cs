namespace EShopper.Domain
{
    public class Country : Entity
    {
        public string Name { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
