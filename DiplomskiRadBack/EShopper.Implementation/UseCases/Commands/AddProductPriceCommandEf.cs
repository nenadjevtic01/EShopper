using FluentValidation;
using FluentValidation.Results;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;
using EShopper.Domain;
using EShopper.Implementation.Validations;

namespace EShopper.Implementation.UseCases.Commands
{
    public class AddProductPriceCommandEf : UseCaseEf, IAddProductPriceCommand
    {
        private IUser _user;
        private AddPriceValidator _validator;
        public int UseCaseId => 19;

        public string UseCaseName => "Add product price";

        public string UseCaseDescription => "Add new product price";

        public AddProductPriceCommandEf(EShopperDbContext context, IUser user, AddPriceValidator validator) : base(context)
        {
            _user = user;
            _validator = validator;
        }

        public void Execute(CreatePriceDto request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User can't add price.");
            }

            _validator.ValidateAndThrow(request);

            var now = DateTime.UtcNow;

            if (PromotionRules.BasePrice(Context, request.ProductId, now) == request.Price)
            {
                throw new ValidationException(new[] { new ValidationFailure("Price", "The new price is the same as the current price.") });
            }

            Context.Prices.Add(new Price
            {
                ProductId = request.ProductId,
                NewPrice = request.Price,
                OldPrice = null,
                ActiveFrom = now.AddMilliseconds(-1)
            });

            PromotionRules.Reprice(Context, request.ProductId, request.Price, now);

            Context.SaveChanges();
        }
    }
}
