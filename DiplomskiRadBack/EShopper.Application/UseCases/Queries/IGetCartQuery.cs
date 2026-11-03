using EShopper.Application.UseCases.DTO;

namespace EShopper.Application.UseCases.Queries
{
    public interface IGetCartQuery : IQuery<int, CartDto>
    {
    }
}
