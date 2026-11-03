using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.Commands;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Commands
{
    public class DeletePaymentMethodCommandEf : UseCaseEf, IDeletePaymentMethodCommand
    {
        private readonly IUser _user;

        public int UseCaseId => 54;

        public string UseCaseName => "Delete payment method";

        public string UseCaseDescription => "Toggle payment method active status.";

        public DeletePaymentMethodCommandEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public void Execute(int request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User don't have permission to delete payment methods.");
            }

            var paymentMethod = Context.PaymentMethods.FirstOrDefault(x => x.Id == request);

            if (paymentMethod == null)
            {
                throw new EntityNotExistException(nameof(PaymentMethod), request);
            }

            paymentMethod.IsActive = !paymentMethod.IsActive;
            Context.PaymentMethods.Update(paymentMethod);
            Context.SaveChanges();
        }
    }
}
