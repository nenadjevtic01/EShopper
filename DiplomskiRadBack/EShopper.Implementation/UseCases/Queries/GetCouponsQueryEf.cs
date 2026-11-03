using EShopper.Application.UseCases.DTO;
using EShopper.Application.UseCases.Queries;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Queries
{
    public class GetCouponsQueryEf : UseCaseEf, IGetCouponsQuery
    {
        private readonly IUser _user;

        public int UseCaseId => 43;

        public string UseCaseName => "Get coupons";

        public string UseCaseDescription => "Get coupons with number of uses.";

        public GetCouponsQueryEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public PageResponse<CouponDto> Execute(PagedSearchDto request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User don't have permission to view coupons.");
            }

            var query = Context.Coupons.Where(x => request.IncludeInactive == true || x.IsActive).AsQueryable();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.Code.Contains(request.Keyword));
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

            var response = new PageResponse<CouponDto>();
            response.Count = query.Count();
            response.Data = query.Skip(toSkip).Take(request.PerPage.Value).Select(x => new CouponDto
            {
                Id = x.Id,
                Code = x.Code,
                DiscountPercentage = x.DiscountPercentage,
                ValidUntil = x.ValidUntil,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                TimesUsed = x.Usages.Count(),
                Usages = x.Usages.Select(u => new CouponUsageDto
                {
                    UserId = u.UserId,
                    UserName = u.User.FirstName + " " + u.User.LastName,
                    UserEmail = u.User.Email,
                    ReceiptId = u.ReceiptId,
                    UsedAt = u.CreatedAt
                }).OrderByDescending(u => u.UsedAt).ToList()
            }).ToList();
            response.CurrentPage = request.Page.Value;
            response.PerPage = request.PerPage.Value;

            return response;
        }
    }
}
