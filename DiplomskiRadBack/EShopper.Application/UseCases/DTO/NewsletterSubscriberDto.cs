namespace EShopper.Application.UseCases.DTO
{
    public class NewsletterSubscriberDto : BaseDto
    {
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class SubscribeNewsletterDto
    {
        public string Email { get; set; }
    }
}
