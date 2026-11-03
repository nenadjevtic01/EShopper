namespace EShopper.Application.UseCases.DTO
{
    public class UpdateProductDto
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public int GenderId { get; set; }
        public int CountryId { get; set; }
        public bool InStock { get; set; }
        public string? Material { get; set; }

        public IEnumerable<PictureDto>? Pictures { get; set; }
        public IEnumerable<ProductSizeQuantityDto>? Sizes { get; set; }
    }
}
