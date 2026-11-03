namespace EShopper.Domain
{
    public class ContactMessage : Entity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string? Answer { get; set; }
        public bool IsAnswered { get; set; }
        public DateTime? AnsweredAt { get; set; }
    }
}
