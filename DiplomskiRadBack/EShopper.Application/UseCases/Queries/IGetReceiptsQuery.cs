using EShopper.Application.UseCases.DTO;

namespace EShopper.Application.UseCases.Queries
{
    public interface IGetReceiptsQuery : IQuery<SearchReceiptDto, PageResponse<ReceiptDto>>
    {
    }
}
