namespace EShopper.Domain
{
    public class HeroSlide : Entity
    {
        public string? Tagline { get; set; }
        public string Title { get; set; }
        public string? Subtitle { get; set; }
        public string? ButtonText { get; set; }
        public string? ButtonLink { get; set; }
        public string MediaType { get; set; }
        public string? MediaSrc { get; set; }
        public int SortOrder { get; set; }
    }
}
