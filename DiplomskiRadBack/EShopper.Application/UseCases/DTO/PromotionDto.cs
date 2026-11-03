namespace EShopper.Application.UseCases.DTO
{
    public class PromotionDto : BaseDto
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ProductCount { get; set; }
        public IEnumerable<PromotionGroupDto> Groups { get; set; }
    }

    public class PromotionGroupDto
    {
        public int DiscountPercentage { get; set; }
        public IEnumerable<PromotionGroupProductDto> Products { get; set; }
    }

    public class PromotionGroupProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public PictureDto Picture { get; set; }
    }

    public class CreatePromotionDto
    {
        public string Name { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }
        public IEnumerable<CreatePromotionGroupDto> Groups { get; set; }
    }

    public class UpdatePromotionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }
        public IEnumerable<CreatePromotionGroupDto> Groups { get; set; }
    }

    public class CreatePromotionGroupDto
    {
        public int DiscountPercentage { get; set; }
        public IEnumerable<int> ProductIds { get; set; }
    }

    public class ActivePromotionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<PromotionProductCardDto> Products { get; set; }
    }

    public class PromotionProductCardDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public decimal NewPrice { get; set; }
        public decimal? OldPrice { get; set; }
        public int DiscountPercentage { get; set; }
        public bool InStock { get; set; }
        public IEnumerable<PictureDto> Picture { get; set; }
    }
}
