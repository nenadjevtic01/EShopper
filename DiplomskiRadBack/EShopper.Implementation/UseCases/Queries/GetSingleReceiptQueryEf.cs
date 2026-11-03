using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.DTO;
using EShopper.Application.UseCases.Queries;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Queries
{
    public class GetSingleReceiptQueryEf : UseCaseEf, IGetSingleReceiptQuery
    {
        private IUser _user;
        public int UseCaseId => 14;

        public string UseCaseName => "Get single receipt.";

        public string UseCaseDescription => "Get single receipt informations.";

        public GetSingleReceiptQueryEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public ReceiptDto Execute(int request)
        {
            var id = _user.Id;
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                var ownerId = Context.Receipts.Where(x => x.Id == request).Select(x => (int?)x.UserId).FirstOrDefault();

                if (ownerId != id)
                {
                    throw new ForbiddenQueryException();
                }
            }

            var receipt = Context.Receipts.Where(x => x.Id == request).FirstOrDefault();

            if (receipt == null)
            {
                throw new EntityNotExistException(nameof(Receipt), request);
            }

            return new ReceiptDto
            {
                ReceiptId = receipt.Id,
                Subtotal = receipt.Subtotal,
                Total = receipt.Total,
                ShippingFee = receipt.ShippingFee,
                IsActive = receipt.IsActive,
                OrderedAt = receipt.CreatedAt,
                CustomerId = receipt.UserId,
                CustomerName = receipt.User.FirstName + " " + receipt.User.LastName,
                CustomerEmail = receipt.User.Email,
                PaymentMethodId = receipt.PaymentMethodId,
                PaymentMethodName = receipt.PaymentMethod.Name,
                BillingFirstName = receipt.BillingFirstName,
                BillingLastName = receipt.BillingLastName,
                BillingEmail = receipt.BillingEmail,
                BillingAddress = receipt.BillingAddress,
                BillingCity = receipt.BillingCity,
                BillingCountry = receipt.BillingCountry,
                BillingPostalCode = receipt.BillingPostalCode,
                CouponCode = receipt.Coupon != null ? receipt.Coupon.Code : null,
                DiscountAmount = receipt.DiscountAmount,
                Items = receipt.ReceiptItems.Select(y => new ReceiptItemDto
                {
                    ReceiptItemId = y.Id,
                    ProductId = y.ProductId,
                    ProductName = y.Product.ProductName,
                    Picture = y.Product.Pictures.Select(p => new PictureDto { Src = p.Src, Alt = p.Alt }).FirstOrDefault(),
                    Price = y.Price.NewPrice,
                    Category = y.Product.Category.CategoryName,
                    Brand = y.Product.Brand.BrandName,
                    Gender = y.Product.Gender.GenderName,
                    Size = y.Size.SizeName,
                    Quantity = y.Quantity,
                    MyRating = Context.ProductRatings
                        .Where(r => r.ProductId == y.ProductId && r.UserId == id)
                        .Select(r => (decimal?)r.Rating)
                        .FirstOrDefault(),
                    MyComment = Context.ProductRatings
                        .Where(r => r.ProductId == y.ProductId && r.UserId == id)
                        .Select(r => r.Comment)
                        .FirstOrDefault()
                })
            };
        }
    }
}
