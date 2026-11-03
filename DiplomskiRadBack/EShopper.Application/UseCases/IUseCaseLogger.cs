using EShopper.Application.UseCases.DTO;

namespace EShopper.Application.UseCases
{
    public interface IUseCaseLogger
    {
        void Log(UseCaseLog log);
    }
}
