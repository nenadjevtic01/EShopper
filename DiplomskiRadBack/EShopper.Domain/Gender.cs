namespace EShopper.Domain
{
    public class Gender : Entity
    {
        public string GenderName { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
