using EShopper.Domain;

namespace EShopper.Api.Core
{
    public class JwtUser : IUser
    {
        public string Identity { get; set; }

        public int Id { get; set; }
        public IEnumerable<int> UseCaseIds { get; set; } = new List<int>();
        public string Email { get; set; }
        public int Role { get; set; }
    }

    public class AnonimousUser : IUser
    {
        public string Identity => "Anonymous";

        public int Id => 0;

        public IEnumerable<int> UseCaseIds => new List<int> { 18, 5, 7, 4, 23, 24, 1, 26, 33, 34, 38, 50, 51, 55, 56, 57, 58 };

        public string Email => "anonimous@gmail.com";

        public int Role => 0;
    }
}
