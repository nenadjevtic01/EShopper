using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.Commands;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Commands
{
    public class DeleteCouponCommandEf : UseCaseEf, IDeleteCouponCommand
    {
        private readonly IUser _user;

        public int UseCaseId => 42;

        public string UseCaseName => "Delete coupon";

        public string UseCaseDescription => "Toggle coupon active status.";

        public DeleteCouponCommandEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public void Execute(int request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User don't have permission to delete coupons.");
            }

            var coupon = Context.Coupons.FirstOrDefault(x => x.Id == request);

            if (coupon == null)
            {
                throw new EntityNotExistException(nameof(Coupon), request);
            }

            coupon.IsActive = !coupon.IsActive;
            Context.Coupons.Update(coupon);
            Context.SaveChanges();
        }
    }
}
