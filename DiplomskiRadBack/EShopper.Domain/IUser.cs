namespace EShopper.Domain
{
    public interface IUser
    {
        public string Identity { get; }
        public int Id { get; }
        public IEnumerable<int> UseCaseIds { get; }
        public string Email { get; }
        public int Role { get; }
    }
}
