namespace EShopper.Application.UseCases.DTO
{
    public class UserDtoResponse
    {
        public int? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? AvatarSrc { get; set; }

        public string? Password { get; set; }
        public bool? IsBanned { get; set; }
        public int? Role { get; set; }

        public InfoDto? Info { get; set; }

        public CartDto? Cart { get; set; }

        public IEnumerable<UseCaseDto>? UseCases { get; set; }
    }
}
