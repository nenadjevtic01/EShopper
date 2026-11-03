using EShopper.Application.UseCases.DTO;

namespace EShopper.Application.UseCases.Commands
{
    public interface IConfirmOrderCommand : EShopper.Application.UseCases.IQuery<ConfirmOrderDto, int>
    {
    }
}
