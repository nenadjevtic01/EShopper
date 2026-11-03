namespace EShopper.Application.UseCases.DTO
{
    public class BrandDto : BaseDto
    {
        public string Name { get; set; }
        public PictureDto Picture { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateBrandDto
    {
        public string Name { get; set; }
        public PictureDto Picture { get; set; }
    }

    public class UpdateBrandDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public PictureDto Picture { get; set; }
    }
}
