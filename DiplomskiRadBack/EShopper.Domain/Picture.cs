namespace EShopper.Domain
{
    public class Picture : Entity
    {
        public string Src { get; set; }
        public string Alt { get; set; }
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }
    }
}
