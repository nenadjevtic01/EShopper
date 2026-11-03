namespace EShopper.Application.UseCases.DTO
{
    public class CategoryDto : BaseDto
    {
        public string Name { get; set; }
        public PictureDto Picture { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<ProductDto> Products { get; set; }
    }

    public class CreateCategoryDto
    {
        public string Name { get; set; }
        public PictureDto Picture { get; set; }
    }
}
