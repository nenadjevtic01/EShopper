namespace EShopper.Application.UseCases.DTO
{
    public class CartDto
    {
        public int? CartId { get; set; }
        public IEnumerable<CartItemDto>? Items { get; set; }
    }
}
