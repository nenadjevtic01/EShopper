namespace EShopper.Application.UseCases.DTO
{
    public class SizeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
    }

    public class ProductSizeQuantityDto
    {
        public int SizeId { get; set; }
        public int Quantity { get; set; }
    }
}
