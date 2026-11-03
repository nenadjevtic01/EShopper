namespace EShopper.Application.UseCases.DTO
{
    public class SearchProductDto : PagedSearchDto
    {
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public decimal? MinimumPrice { get; set; }
        public decimal? MaximumPrice { get; set; }

        public List<int>? CategoryIds { get; set; }
        public List<int>? BrandIds { get; set; }
        public List<int>? GenderIds { get; set; }
        public List<int>? SizeIds { get; set; }

        public bool? OnlyInStock { get; set; }

        public string? SortBy { get; set; }

        public string? SortDirection { get; set; }
    }
}
