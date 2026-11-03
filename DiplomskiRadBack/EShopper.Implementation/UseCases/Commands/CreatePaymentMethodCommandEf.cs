using FluentValidation;
using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Commands
{
    public class CreatePaymentMethodCommandEf : UseCaseEf, ICreatePaymentMethodCommand
    {
        private readonly IUser _user;

        public int UseCaseId => 52;

        public string UseCaseName => "Create payment method";

        public string UseCaseDescription => "Create new payment method.";

        public CreatePaymentMethodCommandEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public void Execute(CreatePaymentMethodDto request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User don't have permission to create payment methods.");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ValidationException("Payment method name is required.");
            }

            var name = request.Name.Trim();

            if (Context.PaymentMethods.Any(x => x.Name == name))
            {
                throw new ItemAlreadyExistException(nameof(PaymentMethod));
            }

            Context.PaymentMethods.Add(new PaymentMethod
            {
                Name = name
            });

            Context.SaveChanges();
        }
    }
}
