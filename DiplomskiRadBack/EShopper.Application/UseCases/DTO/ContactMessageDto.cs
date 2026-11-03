namespace EShopper.Application.UseCases.DTO
{
    public class ContactMessageDto : BaseDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string Answer { get; set; }
        public bool IsAnswered { get; set; }
        public DateTime? AnsweredAt { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateContactMessageDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }

    public class AnswerContactMessageDto
    {
        public int Id { get; set; }
        public string Answer { get; set; }
    }

    public class SearchContactMessageDto : PagedSearchDto
    {
        public bool? IsAnswered { get; set; }
    }
}
