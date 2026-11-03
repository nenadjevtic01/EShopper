using EShopper.Application.UseCases.DTO;

namespace EShopper.Application.UseCases.Queries
{
    public interface IGetSingleReceiptQuery : IQuery<int, ReceiptDto>
    {
    }
}
