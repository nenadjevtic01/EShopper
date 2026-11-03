using System.Text.RegularExpressions;
using EShopper.Application.UseCases.DTO;
using EShopper.Application.UseCases.Queries;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Queries
{
    public class GetAuditLogsQueryEf : UseCaseEf, IGetAuditLogsQuery
    {
        private const int MaxDataLength = 2000;

        private static readonly Regex SecretFields = new Regex(
            "\"(?<key>[^\"]*(?:password|token)[^\"]*)\"\\s*:\\s*\"[^\"]*\"",
            RegexOptions.IgnoreCase);

        private readonly IUser _user;

        public int UseCaseId => 63;

        public string UseCaseName => "Get audit logs";

        public string UseCaseDescription => "Get audit log entries.";

        public GetAuditLogsQueryEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public PageResponse<AuditLogDto> Execute(SearchAuditLogDto request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User don't have permission to view the audit log.");
            }

            var query = Context.AuditLogs.AsQueryable();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.UseCaseName.Contains(request.Keyword));
            }

            if (request.IsAuthorized != null)
            {
                query = query.Where(x => x.IsAuthorized == request.IsAuthorized);
            }

            if (!string.IsNullOrEmpty(request.User))
            {
                query = query.Where(x => x.Name == request.User);
            }

            var descending = request.SortDirection != "asc";

            switch (request.SortBy)
            {
                case "id":
                    query = descending ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id);
                    break;

                case "user":
                    query = descending ? query
                        .OrderByDescending(x => x.Name)
                        .ThenByDescending(x => x.Id) : query
                        .OrderBy(x => x.Name)
                        .ThenBy(x => x.Id);
                    break;

                case "action":
                    query = descending ? query
                        .OrderByDescending(x => x.UseCaseName)
                        .ThenByDescending(x => x.Id) : query
                        .OrderBy(x => x.UseCaseName)
                        .ThenBy(x => x.Id);
                    break;

                case "result":
                    query = descending ? query
                        .OrderByDescending(x => x.IsAuthorized)
                        .ThenByDescending(x => x.Id) : query
                        .OrderBy(x => x.IsAuthorized)
                        .ThenBy(x => x.Id);
                    break;

                default:
                    query = descending ? query
                        .OrderByDescending(x => x.ExecutionDate)
                        .ThenByDescending(x => x.Id) : query
                        .OrderBy(x => x.ExecutionDate)
                        .ThenBy(x => x.Id);
                    break;
            }

            if (request.PerPage == null || request.PerPage < 1)
            {
                request.PerPage = 15;
            }

            if (request.Page == null || request.Page < 1)
            {
                request.Page = 1;
            }

            var toSkip = (request.Page.Value - 1) * request.PerPage.Value;

            var response = new PageResponse<AuditLogDto>();
            response.Count = query.Count();
            response.Data = query.Skip(toSkip).Take(request.PerPage.Value).Select(x => new AuditLogDto
            {
                Id = x.Id,
                UserId = x.UserId,
                User = x.Name,
                UseCaseName = x.UseCaseName,
                ExecutionDate = x.ExecutionDate,
                IsAuthorized = x.IsAuthorized,
                Data = x.Data
            }).ToList();

            foreach (var log in response.Data)
            {
                log.Data = Clean(log.Data);
            }

            response.CurrentPage = request.Page.Value;
            response.PerPage = request.PerPage.Value;

            return response;
        }

        private static string Clean(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return data;
            }

            data = SecretFields.Replace(data, "\"${key}\":\"********\"");

            return data.Length > MaxDataLength ? data.Substring(0, MaxDataLength) + "..." : data;
        }
    }
}
