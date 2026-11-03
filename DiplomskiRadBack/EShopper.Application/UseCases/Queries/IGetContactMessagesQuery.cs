using EShopper.Application.UseCases.DTO;

namespace EShopper.Application.UseCases.Queries
{
    public interface IGetContactMessagesQuery : IQuery<SearchContactMessageDto, PageResponse<ContactMessageDto>>
    {
    }
}
