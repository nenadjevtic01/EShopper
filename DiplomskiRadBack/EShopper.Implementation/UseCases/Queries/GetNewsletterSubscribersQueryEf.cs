using EShopper.Application.UseCases.DTO;
using EShopper.Application.UseCases.Queries;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Queries
{
    public class GetNewsletterSubscribersQueryEf : UseCaseEf, IGetNewsletterSubscribersQuery
    {
        private readonly IUser _user;

        public int UseCaseId => 39;

        public string UseCaseName => "Get newsletter subscribers";

        public string UseCaseDescription => "Get list of newsletter subscribers.";

        public GetNewsletterSubscribersQueryEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public PageResponse<NewsletterSubscriberDto> Execute(PagedSearchDto request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User don't have permission to view newsletter subscribers.");
            }

            var query = Context.NewsletterSubscribers.Where(x => request.IncludeInactive == true || x.IsActive).AsQueryable();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.Email.Contains(request.Keyword));
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

            var response = new PageResponse<NewsletterSubscriberDto>();
            response.Count = query.Count();
            response.Data = query.Skip(toSkip).Take(request.PerPage.Value).Select(x => new NewsletterSubscriberDto
            {
                Id = x.Id,
                Email = x.Email,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt
            }).ToList();
            response.CurrentPage = request.Page.Value;
            response.PerPage = request.PerPage.Value;

            return response;
        }
    }
}
