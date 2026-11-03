using FluentValidation;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;

namespace EShopper.Implementation.Validations
{
    public class AddPriceValidator : AbstractValidator<CreatePriceDto>
    {
        private EShopperDbContext _context;

        public AddPriceValidator(EShopperDbContext context)
        {
            _context = context;

            RuleFor(x => x.ProductId)
                .Must(id => _context.Products.Any(x => x.Id == id)).WithMessage("Product with provided id does not exist.");

            RuleFor(x => x.Price).Cascade(CascadeMode.Stop)
                .GreaterThan(0).WithMessage("Price must be greater than 0.")
                .LessThan(100000).WithMessage("Price must be less than 100000.")
                .Must(price => decimal.Round(price, 2) == price).WithMessage("Price can have at most 2 decimals.");
        }
    }
}
