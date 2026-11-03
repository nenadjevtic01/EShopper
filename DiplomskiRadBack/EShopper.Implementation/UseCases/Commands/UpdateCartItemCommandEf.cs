using FluentValidation;
using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;
using EShopper.Domain;
using EShopper.Implementation.Validations;

namespace EShopper.Implementation.UseCases.Commands
{
    public class UpdateCartItemCommandEf : UseCaseEf, IUpdateCartItemCommand
    {
        private readonly UpdateCartItemValidator _validator;
        private IUser _user;
        public int UseCaseId => 10;

        public string UseCaseName => "Update cart item.";

        public string UseCaseDescription => "Update cart item details.";

        public UpdateCartItemCommandEf(EShopperDbContext context, UpdateCartItemValidator validator, IUser user) : base(context)
        {
            _validator = validator;
            _user = user;
        }

        public void Execute(UpdateCartItemDto request)
        {
            var id = _user.Id;
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 1)
            {
                throw new CustomerOnlyException("Admins can't change a cart.");
            }

            var cartOwnerId = Context.Carts.Where(x => x.Id == request.CartId).Select(x => (int?)x.UserId).FirstOrDefault();

            if (cartOwnerId != id)
            {
                throw new Exception("Can't update someone else cart.");
            }

            _validator.ValidateAndThrow(request);

            var cartItem = Context.CartItems.Where(x => x.Id == request.CartItemId).FirstOrDefault();
            var productSizes = Context.ProductSizes.Where(x => x.ProductId == request.ProductId).Select(x => x.SizeId);

            if (cartItem == null)
            {
                throw new EntityNotExistException(nameof(CartItem), request.CartItemId);
            }

            if (cartItem.ProductId != request.ProductId)
            {
                throw new Exception("Product id is not the same as the product id of cart item");
            }

            if (cartItem.CartId != request.CartId)
            {
                throw new Exception("Cart id is not the same as the cart id of cart item");
            }

            if (!productSizes.Contains(request.SizeId))
            {
                throw new Exception("Product does not contain size id provided");
            }

            var availableStock = Context.ProductSizes
                .Where(x => x.ProductId == request.ProductId && x.SizeId == request.SizeId)
                .Select(x => (int?)x.Quantity)
                .FirstOrDefault() ?? 0;

            if (cartItem.SizeId == request.SizeId)
            {
                if (request.Quantity > cartItem.Quantity && request.Quantity > availableStock)
                {
                    throw new Exception(availableStock == 0
                        ? "This size is out of stock."
                        : $"Only {availableStock} available for this size.");
                }

                cartItem.Quantity = request.Quantity;
                cartItem.TotalPrice = request.Quantity * Context.Prices
                    .OrderByDescending(x => x.ActiveFrom)
                    .Where(x => x.ActiveFrom < DateTime.UtcNow && x.ProductId == request.ProductId)
                    .Select(x => x.NewPrice)
                    .First();
                Context.CartItems.Update(cartItem);
            }
            else
            {
                var alreadyInCartForTargetSize = Context.CartItems
                    .Where(x => x.CartId == request.CartId && x.ProductId == request.ProductId && x.SizeId == request.SizeId)
                    .Select(x => (int?)x.Quantity)
                    .FirstOrDefault() ?? 0;

                if (alreadyInCartForTargetSize + request.Quantity > availableStock)
                {
                    var canAdd = Math.Max(0, availableStock - alreadyInCartForTargetSize);
                    throw new Exception(availableStock == 0
                        ? "This size is out of stock."
                        : canAdd == 0
                            ? $"You already have all {availableStock} available in your cart."
                            : $"Only {canAdd} more can be added for this size - {availableStock} in stock.");
                }

                var newItem = Context.CartItems
                    .Where(x => x.CartId == request.CartId && x.ProductId == request.ProductId && x.SizeId == request.SizeId)
                    .FirstOrDefault();

                if (newItem == null)
                {
                    var newItemToAdd = new CartItem
                    {
                        CartId = request.CartId,
                        ProductId = request.ProductId,
                        SizeId = request.SizeId,
                        Quantity = request.Quantity,
                        TotalPrice = request.Quantity * Context.Prices
                            .OrderByDescending(x => x.ActiveFrom)
                            .Where(x => x.ActiveFrom < DateTime.UtcNow && x.ProductId == request.ProductId)
                            .Select(x => x.NewPrice)
                            .First()
                    };

                    Context.CartItems.Add(newItemToAdd);
                }
                else
                {
                    newItem.Quantity = newItem.Quantity + request.Quantity;
                    newItem.TotalPrice = newItem.Quantity * Context.Prices
                        .OrderByDescending(x => x.ActiveFrom)
                        .Where(x => x.ActiveFrom < DateTime.UtcNow && x.ProductId == request.ProductId)
                        .Select(x => x.NewPrice)
                        .First();
                    Context.CartItems.Update(newItem);
                }

                Context.CartItems.Remove(cartItem);
            }

            Context.SaveChanges();
        }
    }
}
