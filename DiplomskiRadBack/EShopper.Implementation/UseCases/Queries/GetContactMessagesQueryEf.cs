using EShopper.Application.UseCases.DTO;
using EShopper.Application.UseCases.Queries;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Queries
{
    public class GetContactMessagesQueryEf : UseCaseEf, IGetContactMessagesQuery
    {
        private readonly IUser _user;

        public int UseCaseId => 36;

        public string UseCaseName => "Get contact messages";

        public string UseCaseDescription => "Get contact messages for admin.";

        public GetContactMessagesQueryEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public PageResponse<ContactMessageDto> Execute(SearchContactMessageDto request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User don't have permission to view contact messages.");
            }

            var query = Context.ContactMessages.Where(x => request.IncludeInactive == true || x.IsActive).AsQueryable();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.Name.Contains(request.Keyword)
                    || x.Email.Contains(request.Keyword)
                    || x.Subject.Contains(request.Keyword));
            }

            if (request.IsAnswered != null)
            {
                query = query.Where(x => x.IsAnswered == request.IsAnswered);
            }

            query = query.OrderByDescending(x => x.CreatedAt);

            if (request.PerPage == null || request.PerPage < 1)
            {
                request.PerPage = 15;
            }

            if (request.Page == null || request.Page < 1)
            {
                request.Page = 1;
            }

            var toSkip = (request.Page.Value - 1) * request.PerPage.Value;

            var response = new PageResponse<ContactMessageDto>();
            response.Count = query.Count();
            response.Data = query.Skip(toSkip).Take(request.PerPage.Value).Select(x => new ContactMessageDto
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                Subject = x.Subject,
                Message = x.Message,
                Answer = x.Answer,
                IsAnswered = x.IsAnswered,
                AnsweredAt = x.AnsweredAt,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt
            }).ToList();
            response.CurrentPage = request.Page.Value;
            response.PerPage = request.PerPage.Value;

            return response;
        }
    }
}
