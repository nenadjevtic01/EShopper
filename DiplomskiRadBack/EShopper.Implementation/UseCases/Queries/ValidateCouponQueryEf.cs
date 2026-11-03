using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.DTO;
using EShopper.Application.UseCases.Queries;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Queries
{
    public class ValidateCouponQueryEf : UseCaseEf, IValidateCouponQuery
    {
        private readonly IUser _user;

        public int UseCaseId => 44;

        public string UseCaseName => "Validate coupon";

        public string UseCaseDescription => "Check if coupon can be used.";

        public ValidateCouponQueryEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public CouponValidationResultDto Execute(ValidateCouponDto request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 1)
            {
                throw new CustomerOnlyException("Admins can't place orders.");
            }

            if (string.IsNullOrWhiteSpace(request.Code))
            {
                return new CouponValidationResultDto { Valid = false, Message = "Enter a coupon code." };
            }

            var code = request.Code.Trim().ToUpper();
            var coupon = Context.Coupons.FirstOrDefault(x => x.Code == code);

            if (coupon == null || !coupon.IsActive)
            {
                return new CouponValidationResultDto { Valid = false, Message = "This coupon code does not exist." };
            }

            if (coupon.ValidUntil < DateTime.UtcNow)
            {
                return new CouponValidationResultDto { Valid = false, Message = "This coupon has expired." };
            }

            if (Context.CouponUsages.Any(x => x.CouponId == coupon.Id && x.UserId == _user.Id))
            {
                return new CouponValidationResultDto { Valid = false, Message = "You have already used this coupon." };
            }

            return new CouponValidationResultDto
            {
                Valid = true,
                Message = $"{coupon.DiscountPercentage}% discount applied.",
                DiscountPercentage = coupon.DiscountPercentage
            };
        }
    }
}
