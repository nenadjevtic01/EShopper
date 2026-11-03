using FluentValidation;
using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Commands
{
    public class CreateCouponCommandEf : UseCaseEf, ICreateCouponCommand
    {
        private readonly IUser _user;

        public int UseCaseId => 40;

        public string UseCaseName => "Create coupon";

        public string UseCaseDescription => "Create new discount coupon.";

        public CreateCouponCommandEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public void Execute(CreateCouponDto request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User don't have permission to create coupons.");
            }

            if (string.IsNullOrWhiteSpace(request.Code))
            {
                throw new ValidationException("Coupon code is required.");
            }

            if (request.DiscountPercentage < 1 || request.DiscountPercentage > 100)
            {
                throw new ValidationException("Discount percentage must be between 1 and 100.");
            }

            if (request.ValidUntil <= DateTime.UtcNow)
            {
                throw new ValidationException("Valid until date must be in the future.");
            }

            var code = request.Code.Trim().ToUpper();

            if (Context.Coupons.Any(x => x.Code == code))
            {
                throw new ItemAlreadyExistException(nameof(Coupon));
            }

            Context.Coupons.Add(new Coupon
            {
                Code = code,
                DiscountPercentage = request.DiscountPercentage,
                ValidUntil = request.ValidUntil
            });

            Context.SaveChanges();
        }
    }
}
