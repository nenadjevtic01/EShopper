namespace EShopper.Domain
{
    public class UseCase : Entity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<UserUseCase> UserUseCases { get; set; } = new List<UserUseCase>();
    }
}
