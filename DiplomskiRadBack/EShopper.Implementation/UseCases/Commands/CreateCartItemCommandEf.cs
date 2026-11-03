using FluentValidation;
using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;
using EShopper.Domain;
using EShopper.Implementation.Validations;

namespace EShopper.Implementation.UseCases.Commands
{
    public class CreateCartItemCommandEf : UseCaseEf, ICreateCartItemCommand
    {
        private readonly CreateCartItemValidator _validator;

        private IUser _user;

        public int UseCaseId => 9;

        public string UseCaseName => "Add to cart.";

        public string UseCaseDescription => "Add product to cart.";

        public CreateCartItemCommandEf(EShopperDbContext context, CreateCartItemValidator validator, IUser user) : base(context)
        {
            _validator = validator;
            _user = user;
        }

        public void Execute(CreateCartItemDto request)
        {
            var id = _user.Id;

            var role = Context.Users.Where(x => x.Id == id).Select(x => x.Role).First();

            if (role == 1)
            {
                throw new CustomerOnlyException("Admins can't add items to a cart.");
            }

            _validator.ValidateAndThrow(request);

            var cartId = Context.Carts.Where(x => x.UserId == id).Select(x => x.Id).First();

            if (Context.Carts.Where(x => x.Id == request.CartId).Select(x => x.UserId).First() != id)
            {
                throw new Exception("Can't add to someone else cart.");
            }

            var availableStock = Context.ProductSizes
                .Where(x => x.ProductId == request.ProductId && x.SizeId == request.SizeId)
                .Select(x => (int?)x.Quantity)
                .FirstOrDefault() ?? 0;
            var alreadyInCart = Context.CartItems
                .Where(x => x.CartId == request.CartId && x.ProductId == request.ProductId && x.SizeId == request.SizeId)
                .Select(x => (int?)x.Quantity)
                .FirstOrDefault() ?? 0;

            if (alreadyInCart + request.Quantity > availableStock)
            {
                var canAdd = Math.Max(0, availableStock - alreadyInCart);
                throw new Exception(availableStock == 0
                    ? "This size is out of stock."
                    : canAdd == 0
                        ? $"You already have all {availableStock} available in your cart."
                        : $"Only {canAdd} more can be added for this size - {availableStock} in stock.");
            }

            var price = Context.Prices
                .OrderByDescending(x => x.ActiveFrom)
                .Where(x => x.ProductId == request.ProductId && x.ActiveFrom < DateTime.UtcNow)
                .Select(x => x.NewPrice)
                .First();

            if (Context.CartItems.Any(x => x.CartId == request.CartId && x.ProductId == request.ProductId && x.SizeId == request.SizeId))
            {
                var item = Context.CartItems
                    .Where(x => x.CartId == request.CartId && x.ProductId == request.ProductId && x.SizeId == request.SizeId)
                    .First();
                item.Quantity = item.Quantity + request.Quantity;
                item.TotalPrice = item.Quantity * price;
                Context.CartItems.Update(item);
            }
            else
            {
                var cartItem = new CartItem
                {
                    CartId = cartId,
                    ProductId = request.ProductId,
                    SizeId = request.SizeId,
                    Quantity = request.Quantity,
                    TotalPrice = request.Quantity * price
                };

                Context.CartItems.Add(cartItem);
            }

            Context.SaveChanges();
        }
    }
}
