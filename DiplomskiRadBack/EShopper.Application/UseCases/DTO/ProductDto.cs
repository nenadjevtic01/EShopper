namespace EShopper.Application.UseCases.DTO
{
    public class ProductDto
    {
        public int? Id { get; set; }
        public string? ProductName { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public int? GenderId { get; set; }
        public decimal NewPrice { get; set; }
        public decimal? OldPrice { get; set; }
        public bool? Sale { get; set; }
        public bool? InStock { get; set; }
        public bool? IsActive { get; set; }
        public string? Material { get; set; }
        public int? CountryId { get; set; }
        public string? CountryOfOrigin { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public IEnumerable<PictureDto>? Picture { get; set; }
        public string[]? Sizes { get; set; }
        public IEnumerable<SizeDto>? SizeStocks { get; set; }
    }
}
