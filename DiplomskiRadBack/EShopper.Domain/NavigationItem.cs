namespace EShopper.Domain
{
    public class NavigationItem : Entity
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public int DisplayOrder { get; set; }
    }
}
