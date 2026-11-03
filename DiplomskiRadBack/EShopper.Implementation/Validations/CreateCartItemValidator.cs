using FluentValidation;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;

namespace EShopper.Implementation.Validations
{
    public class CreateCartItemValidator : AbstractValidator<CreateCartItemDto>
    {
        private EShopperDbContext _context;

        public CreateCartItemValidator(EShopperDbContext context)
        {
            _context = context;

            RuleFor(x => x.ProductId).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("Product id is required.")
                .Must(id => _context.Products.Any(x => x.Id == id)).WithMessage("Product with provided id does not exist");

            RuleFor(x => x.SizeId).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("Size id is required.")
                .Must(id => _context.Sizes.Any(x => x.Id == id)).WithMessage("Size with provided id does not exist");

            RuleFor(x => x.Quantity).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("Quantity is required/at least 1.")
                .GreaterThan(0).WithMessage("Quantity must be at least 1");
        }
    }
}
