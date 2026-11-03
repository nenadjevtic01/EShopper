namespace EShopper.Application.UseCases.DTO
{
    public class AdminCreateUserResultDto
    {
        public int UserId { get; set; }
        public string? Email { get; set; }
        public string? ResetLink { get; set; }
        public bool EmailSent { get; set; }
    }
}
