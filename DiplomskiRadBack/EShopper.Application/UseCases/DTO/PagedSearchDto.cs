namespace EShopper.Application.UseCases.DTO
{
    public class PagedSearch
    {
        public int? PerPage { get; set; } = 15;
        public int? Page { get; set; } = 1;

        public bool? IncludeInactive { get; set; }
    }

    public class PagedSearchDto : PagedSearch
    {
        public string? Keyword { get; set; }
    }
}
