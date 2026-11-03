namespace EShopper.Application.UseCases.DTO
{
    public class CartItemDto
    {
        public int? CartId { get; set; }
        public int? CartItemId { get; set; }
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? SizeId { get; set; }
        public string? Size { get; set; }
        public int? Quantity { get; set; }
        public decimal? TotalPrice { get; set; }

        public PictureDto Picture { get; set; }
    }
}
