namespace EShopper.Application.UseCases.DTO
{
    public class ReceiptItemDto
    {
        public int ReceiptItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public PictureDto? Picture { get; set; }
        public decimal Price { get; set; }
        public string Brand { get; set; }
        public string Gender { get; set; }
        public string Category { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }

        public decimal? MyRating { get; set; }
        public string? MyComment { get; set; }
    }
}
