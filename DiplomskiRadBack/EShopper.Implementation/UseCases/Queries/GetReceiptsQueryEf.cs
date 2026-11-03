using FluentValidation;
using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.DTO;
using EShopper.Application.UseCases.Queries;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Queries
{
    public class GetReceiptsQueryEf : UseCaseEf, IGetReceiptsQuery
    {
        private IUser _user;
        public int UseCaseId => 16;

        public string UseCaseName => "Search receipts";

        public string UseCaseDescription => "Search receipts using dates and user id";

        public GetReceiptsQueryEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public PageResponse<ReceiptDto> Execute(SearchReceiptDto request)
        {
            var id = _user.Id;
            var role = Context.Users.Where(x => x.Id == id).Select(x => x.Role).First();

            if (role == 2 && request.UserId == null)
            {
                request.UserId = id;
            }

            if (role == 2 && request.UserId != id)
            {
                throw new ForbiddenQueryException("receipts");
            }

            if (request.UserId != null && !Context.Users.Any(x => x.Id == request.UserId))
            {
                throw new Exception("User with provided id does not exist.");
            }

            var receipts = Context.Receipts.Where(x => request.IncludeInactive == true || x.IsActive).AsQueryable();

            if (request.UserId != null)
            {
                receipts = receipts.Where(x => x.UserId == request.UserId);
            }

            if (request.FromDate != null)
            {
                receipts = receipts.Where(x => x.CreatedAt > request.FromDate);
            }

            if (request.ToDate != null)
            {
                receipts = receipts.Where(x => x.CreatedAt < request.ToDate);
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

            receipts = receipts.OrderByDescending(x => x.CreatedAt);

            var response = new PageResponse<ReceiptDto>();
            response.Count = receipts.Count();
            response.Data = receipts.Skip(toSkip).Take(request.PerPage.Value).Select(x => new ReceiptDto
            {
                ReceiptId = x.Id,
                Subtotal = x.Subtotal,
                ShippingFee = x.ShippingFee,
                Total = x.Total,
                IsActive = x.IsActive,
                OrderedAt = x.CreatedAt,
                CustomerId = x.UserId,
                CustomerName = x.User.FirstName + " " + x.User.LastName,
                CustomerEmail = x.User.Email,
                PaymentMethodId = x.PaymentMethodId,
                PaymentMethodName = x.PaymentMethod.Name,
                BillingFirstName = x.BillingFirstName,
                BillingLastName = x.BillingLastName,
                BillingEmail = x.BillingEmail,
                BillingAddress = x.BillingAddress,
                BillingCity = x.BillingCity,
                BillingCountry = x.BillingCountry,
                BillingPostalCode = x.BillingPostalCode,
                CouponCode = x.Coupon != null ? x.Coupon.Code : null,
                DiscountAmount = x.DiscountAmount,
                Items = x.ReceiptItems.Select(x => new ReceiptItemDto
                {
                    ReceiptItemId = x.Id,
                    ProductId = x.ProductId,
                    ProductName = x.Product.ProductName,
                    Picture = x.Product.Pictures.Select(p => new PictureDto { Src = p.Src, Alt = p.Alt }).FirstOrDefault(),
                    Brand = x.Product.Brand.BrandName,
                    Price = x.Price.NewPrice,
                    Gender = x.Product.Gender.GenderName,
                    Category = x.Product.Category.CategoryName,
                    Size = x.Size.SizeName,
                    Quantity = x.Quantity,
                    MyRating = Context.ProductRatings
                        .Where(r => r.ProductId == x.ProductId && r.UserId == id)
                        .Select(r => (decimal?)r.Rating)
                        .FirstOrDefault(),
                    MyComment = Context.ProductRatings
                        .Where(r => r.ProductId == x.ProductId && r.UserId == id)
                        .Select(r => r.Comment)
                        .FirstOrDefault()
                })
            }).ToList();

            response.CurrentPage = request.Page.Value;
            response.PerPage = request.PerPage.Value;

            return response;
        }
    }
}
