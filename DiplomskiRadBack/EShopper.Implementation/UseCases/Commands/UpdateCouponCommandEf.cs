using FluentValidation;
using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Commands
{
    public class UpdateCouponCommandEf : UseCaseEf, IUpdateCouponCommand
    {
        private readonly IUser _user;

        public int UseCaseId => 41;

        public string UseCaseName => "Update coupon";

        public string UseCaseDescription => "Update coupon discount and expiry date.";

        public UpdateCouponCommandEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public void Execute(UpdateCouponDto request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User don't have permission to update coupons.");
            }

            var coupon = Context.Coupons.FirstOrDefault(x => x.Id == request.Id);

            if (coupon == null)
            {
                throw new EntityNotExistException(nameof(Coupon), request.Id);
            }

            if (string.IsNullOrWhiteSpace(request.Code))
            {
                throw new ValidationException("Coupon code is required.");
            }

            if (request.DiscountPercentage < 1 || request.DiscountPercentage > 100)
            {
                throw new ValidationException("Discount percentage must be between 1 and 100.");
            }

            var code = request.Code.Trim().ToUpper();

            if (Context.Coupons.Any(x => x.Code == code && x.Id != request.Id))
            {
                throw new ItemAlreadyExistException(nameof(Coupon));
            }

            coupon.Code = code;
            coupon.DiscountPercentage = request.DiscountPercentage;
            coupon.ValidUntil = request.ValidUntil;
            Context.Coupons.Update(coupon);
            Context.SaveChanges();
        }
    }
}
