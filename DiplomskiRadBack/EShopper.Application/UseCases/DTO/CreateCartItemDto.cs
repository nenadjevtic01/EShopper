namespace EShopper.Application.UseCases.DTO
{
    public class CreateCartItemDto
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public int Quantity { get; set; }
    }
}
