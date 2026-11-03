namespace EShopper.Application.UseCases.DTO
{
    public class UpdateCartItemDto
    {
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public int Quantity { get; set; }
    }
}
