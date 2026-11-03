namespace EShopper.Application.UseCases.DTO
{
    public class SingleProductDto
    {
        public int? Id { get; set; }
        public string? ProductName { get; set; }
        public int? Brand { get; set; }
        public int? Gender { get; set; }
        public int? Category { get; set; }
        public bool? Sale { get; set; }
        public bool? InStock { get; set; }
        public string? Material { get; set; }
        public string? CountryOfOrigin { get; set; }

        public decimal? NewPrice { get; set; }
        public decimal? OldPrice { get; set; }
        public IEnumerable<PictureDto>? Pictures { get; set; }
        public IEnumerable<SizeDto>? Sizes { get; set; }

        public decimal? AverageRating { get; set; }
        public int RatingsCount { get; set; }
        public IEnumerable<ProductRatingDto>? Ratings { get; set; }
    }
}
