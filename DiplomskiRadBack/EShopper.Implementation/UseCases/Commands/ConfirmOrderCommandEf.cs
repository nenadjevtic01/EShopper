using EShopper.Application.Email;
using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Commands
{
    public class ConfirmOrderCommandEf : UseCaseEf, IConfirmOrderCommand
    {
        private IUser _user;
        private IEmail _sender;
        public int UseCaseId => 15;

        public string UseCaseName => "Confirm order";

        public string UseCaseDescription => "Confirm order using user id";

        public ConfirmOrderCommandEf(EShopperDbContext context, IUser user, IEmail sender) : base(context)
        {
            _user = user;
            _sender = sender;
        }

        public int Execute(ConfirmOrderDto request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 1)
            {
                throw new CustomerOnlyException("Admins can't place orders.");
            }

            if (request.UserId != _user.Id)
            {
                throw new Exception("Can't confirm another user order");
            }

            var cart = Context.Carts.Where(x => x.UserId == request.UserId).FirstOrDefault();
            var user = Context.Users.Where(x => x.Id == request.UserId).FirstOrDefault();

            if (cart == null)
            {
                throw new EntityNotExistException(nameof(User), request.UserId);
            }

            if (!Context.PaymentMethods.Any(x => x.Id == request.PaymentMethodId && x.IsActive))
            {
                throw new Exception("Selected payment method does not exist.");
            }

            if (string.IsNullOrWhiteSpace(request.BillingFirstName) || string.IsNullOrWhiteSpace(request.BillingLastName)
                || string.IsNullOrWhiteSpace(request.BillingEmail) || string.IsNullOrWhiteSpace(request.BillingAddress)
                || string.IsNullOrWhiteSpace(request.BillingCity) || string.IsNullOrWhiteSpace(request.BillingCountry)
                || string.IsNullOrWhiteSpace(request.BillingPostalCode))
            {
                throw new Exception("Billing details are required.");
            }

            if (request.BillingEmail.Length > 100)
            {
                throw new Exception("Billing email can have at most 100 characters.");
            }

            var cartItems = cart.Items;

            if (cartItems.Count() < 1)
            {
                throw new Exception("Cart is empty.");
            }

            foreach (var item in cartItems)
            {
                var availableStock = Context.ProductSizes
                    .Where(x => x.ProductId == item.ProductId && x.SizeId == item.SizeId)
                    .Select(x => (int?)x.Quantity)
                    .FirstOrDefault() ?? 0;

                if (item.Quantity > availableStock)
                {
                    throw new Exception($"Not enough stock for \"{item.Product.ProductName}\" ({item.Size.SizeName}) - only {availableStock} left.");
                }
            }

            Coupon coupon = null;
            decimal discountAmount = 0;

            if (!string.IsNullOrWhiteSpace(request.CouponCode))
            {
                var code = request.CouponCode.Trim().ToUpper();
                coupon = Context.Coupons.FirstOrDefault(x => x.Code == code);

                if (coupon == null || !coupon.IsActive)
                {
                    throw new Exception("This coupon code does not exist.");
                }

                if (coupon.ValidUntil < DateTime.UtcNow)
                {
                    throw new Exception("This coupon has expired.");
                }

                if (Context.CouponUsages.Any(x => x.CouponId == coupon.Id && x.UserId == request.UserId))
                {
                    throw new Exception("You have already used this coupon.");
                }
            }

            var now = DateTime.UtcNow;

            var currentPrices = cartItems.ToDictionary(
                x => x.Id,
                x => x.Product.Prices
                    .Where(p => p.ActiveFrom < now)
                    .OrderByDescending(p => p.ActiveFrom)
                    .First());

            var subtotal = cartItems.Sum(x => x.Quantity * currentPrices[x.Id].NewPrice);
            var shippingFee = subtotal * 0.2m;

            if (coupon != null)
            {
                discountAmount = Math.Round(subtotal * coupon.DiscountPercentage / 100m, 2);
            }

            var receipt = new Receipt
            {
                User = user,
                PaymentMethodId = request.PaymentMethodId,
                Subtotal = subtotal,
                ShippingFee = shippingFee,
                Total = subtotal + shippingFee - discountAmount,
                BillingFirstName = request.BillingFirstName,
                BillingLastName = request.BillingLastName,
                BillingEmail = request.BillingEmail,
                BillingAddress = request.BillingAddress,
                BillingCity = request.BillingCity,
                BillingCountry = request.BillingCountry,
                BillingPostalCode = request.BillingPostalCode,
                Coupon = coupon,
                DiscountAmount = discountAmount
            };

            var receiptItems = cartItems.Select(x => new ReceiptItem
            {
                Product = x.Product,
                Size = x.Size,
                Price = currentPrices[x.Id],
                Quantity = x.Quantity,
                Receipt = receipt
            });

            Context.Receipts.Add(receipt);
            Context.ReceiptsItems.AddRange(receiptItems);
            Context.CartItems.RemoveRange(cartItems);

            var affectedProductIds = cartItems.Select(x => x.ProductId).Distinct().ToList();
            var affectedProductSizes = Context.ProductSizes.Where(x => affectedProductIds.Contains(x.ProductId)).ToList();

            foreach (var item in cartItems)
            {
                var productSize = affectedProductSizes.First(x => x.ProductId == item.ProductId && x.SizeId == item.SizeId);
                productSize.Quantity -= item.Quantity;
            }

            foreach (var productId in affectedProductIds)
            {
                var affectedProduct = Context.Products.First(x => x.Id == productId);
                affectedProduct.InStock = affectedProductSizes.Any(x => x.ProductId == productId && x.Quantity > 0);
            }

            if (coupon != null)
            {
                Context.CouponUsages.Add(new CouponUsage
                {
                    Coupon = coupon,
                    User = user,
                    Receipt = receipt
                });
            }

            Context.SaveChanges();

            try
            {
                _sender.Send(new MessageDto
                {
                    To = receipt.User.Email,
                    Title = "Order confirmed!",
                    Body = "Dear " + receipt.User.Username + "\n Your order is confirmed...."
                });
            }
            catch (Exception)
            {
            }

            return receipt.Id;
        }
    }
}
