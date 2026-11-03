using EShopper.Application.UseCases.DTO;

namespace EShopper.Application.UseCases.Queries
{
    public interface IGetAuditLogsQuery : IQuery<SearchAuditLogDto, PageResponse<AuditLogDto>>
    {
    }
}
