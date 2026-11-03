namespace EShopper.Application.UseCases
{
    public interface ICommand<TRequest> : IUseCase
    {
        public void Execute(TRequest request);
    }
}
