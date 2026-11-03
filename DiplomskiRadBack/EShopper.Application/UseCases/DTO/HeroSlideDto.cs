namespace EShopper.Application.UseCases.DTO
{
    public class HeroSlideDto
    {
        public int Id { get; set; }
        public string Tagline { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string ButtonText { get; set; }
        public string ButtonLink { get; set; }
        public string MediaType { get; set; }
        public string MediaSrc { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateHeroSlideDto
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

    public class UpdateHeroSlideDto : CreateHeroSlideDto
    {
        public int Id { get; set; }
    }
}
