namespace EShopper.Application.UseCases.DTO
{
    public class AuditLogDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string User { get; set; }
        public string UseCaseName { get; set; }
        public DateTime ExecutionDate { get; set; }
        public bool IsAuthorized { get; set; }
        public string Data { get; set; }
    }

    public class SearchAuditLogDto : PagedSearchDto
    {
        public bool? IsAuthorized { get; set; }
        public string? User { get; set; }
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }
    }
}
