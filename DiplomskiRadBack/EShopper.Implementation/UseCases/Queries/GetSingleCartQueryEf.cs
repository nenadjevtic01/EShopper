using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.DTO;
using EShopper.Application.UseCases.Queries;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Queries
{
    public class GetSingleCartQueryEf : UseCaseEf, IGetCartQuery
    {
        private IUser _user;
        public int UseCaseId => 8;

        public string UseCaseName => "Get single cart";

        public string UseCaseDescription => "Get single cart details.";

        public GetSingleCartQueryEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public CartDto Execute(int request)
        {
            var id = _user.Id;

            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 1)
            {
                throw new CustomerOnlyException("Admins don't have a cart.");
            }

            if (role == 2)
            {
                if (id != request)
                {
                    throw new ForbiddenQueryException("cart");
                }
            }

            var cart = Context.Carts.Where(x => x.UserId == request).FirstOrDefault();

            if (cart == null)
            {
                throw new EntityNotExistException(nameof(Cart), request);
            }

            return new CartDto
            {
                CartId = cart.Id,
                Items = cart.Items.Select(x => new CartItemDto
                {
                    CartId = cart.Id,
                    CartItemId = x.Id,
                    ProductId = x.ProductId,
                    ProductName = x.Product.ProductName,
                    Quantity = x.Quantity,
                    SizeId = x.SizeId,
                    Size = x.Size.SizeName,
                    TotalPrice = x.Quantity * x.Product.Prices
                        .OrderByDescending(x => x.ActiveFrom)
                        .Where(x => x.ActiveFrom < DateTime.UtcNow)
                        .Select(x => x.NewPrice)
                        .First(),
                    Picture = new PictureDto
                    {
                        Src = x.Product.Pictures.First().Src,
                        Alt = x.Product.Pictures.First().Alt
                    }
                })
            };
        }
    }
}
