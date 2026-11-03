namespace EShopper.Application.UseCases.DTO
{
    public class ProductRatingDto
    {
        public decimal Rating { get; set; }
        public string? Comment { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class RateProductDto
    {
        public int ProductId { get; set; }
        public decimal Rating { get; set; }
        public string? Comment { get; set; }
    }
}
