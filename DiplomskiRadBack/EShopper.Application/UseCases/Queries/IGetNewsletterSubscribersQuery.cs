using EShopper.Application.UseCases.DTO;

namespace EShopper.Application.UseCases.Queries
{
    public interface IGetNewsletterSubscribersQuery : IQuery<PagedSearchDto, PageResponse<NewsletterSubscriberDto>>
    {
    }
}
