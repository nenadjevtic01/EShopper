using Microsoft.EntityFrameworkCore;
using EShopper.Application.UseCases.DTO;
using EShopper.Application.UseCases.Queries;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Queries
{
    public class GetUserQueryEf : UseCaseEf, IGetUserQuery
    {
        private IUser _user;
        public int UseCaseId => 3;

        public string UseCaseName => "Get users";

        public string UseCaseDescription => "Get users with/without keyword.";

        public GetUserQueryEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public PageResponse<UserDtoResponse> Execute(PagedSearchDto request)
        {
            var id = _user.Id;

            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            var query = Context.Users
                .Include(x => x.Info)
                .Include(x => x.Cart)
                .Include(x => x.Cart)
                .ThenInclude(x => x.Items)
                .Include(x => x.Cart)
                .ThenInclude(x => x.Items)
                .ThenInclude(x => x.Product)
                .Include(x => x.Cart)
                .ThenInclude(x => x.Items)
                .ThenInclude(x => x.Size)
                .Where(x => request.IncludeInactive == true || !x.IsBanned)
                .AsQueryable();

            if (role == 2)
            {
                query = query.Where(x => x.Id == id);
            }
            else
            {
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    query = query.Where(x =>
                        x.FirstName.Contains(request.Keyword)
                        || x.LastName.Contains(request.Keyword)
                        || x.Username.Contains(request.Keyword));
                }
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

            var response = new PageResponse<UserDtoResponse>();
            response.Count = query.Count();
            response.Data = query.Skip(toSkip).Take(request.PerPage.Value).Select(x => new UserDtoResponse
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                Username = x.Username,
                IsBanned = x.IsBanned,
                Role = x.Role,
                AvatarSrc = x.AvatarSrc,
                Info = new InfoDto
                {
                    Id = x.Info.Id,
                    Address = x.Info.Address,
                    City = x.Info.City,
                    PostalCode = x.Info.PostalCode,
                    Country = x.Info.Country
                },
                Cart = new CartDto
                {
                    CartId = x.Cart.Id,
                    Items = x.Cart.Items.Select(y => new CartItemDto
                    {
                        CartItemId = y.Id,
                        ProductName = y.Product.ProductName,
                        Size = y.Size.SizeName,
                        Quantity = y.Quantity,
                        TotalPrice = y.Quantity * y.Product.Prices
                            .OrderByDescending(y => y.ActiveFrom)
                            .Where(y => y.ActiveFrom < DateTime.UtcNow)
                            .Select(y => y.NewPrice)
                            .First()
                    })
                },
                UseCases = x.UserUseCases.Select(y => new UseCaseDto
                {
                    Id = y.UseCaseId,
                    UseCaseName = y.UseCase.Name
                })
            }).ToList();
            response.CurrentPage = request.Page.Value;
            response.PerPage = request.PerPage.Value;

            return response;
        }
    }
}
