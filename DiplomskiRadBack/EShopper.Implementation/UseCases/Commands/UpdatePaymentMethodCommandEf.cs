using FluentValidation;
using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Commands
{
    public class UpdatePaymentMethodCommandEf : UseCaseEf, IUpdatePaymentMethodCommand
    {
        private readonly IUser _user;

        public int UseCaseId => 53;

        public string UseCaseName => "Update payment method";

        public string UseCaseDescription => "Update payment method name.";

        public UpdatePaymentMethodCommandEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public void Execute(UpdatePaymentMethodDto request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User don't have permission to update payment methods.");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ValidationException("Payment method name is required.");
            }

            var paymentMethod = Context.PaymentMethods.FirstOrDefault(x => x.Id == request.Id);

            if (paymentMethod == null)
            {
                throw new EntityNotExistException(nameof(PaymentMethod), request.Id);
            }

            var name = request.Name.Trim();

            if (Context.PaymentMethods.Any(x => x.Name == name && x.Id != request.Id))
            {
                throw new ItemAlreadyExistException(nameof(PaymentMethod));
            }

            paymentMethod.Name = name;
            Context.PaymentMethods.Update(paymentMethod);
            Context.SaveChanges();
        }
    }
}
