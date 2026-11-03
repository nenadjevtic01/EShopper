using FluentValidation;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;

namespace EShopper.Implementation.Validations
{
    public class CreateBrandValidator : AbstractValidator<CreateBrandDto>
    {
        private EShopperDbContext _context;

        public CreateBrandValidator(EShopperDbContext context)
        {
            _context = context;

            RuleFor(x => x.Name).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(2).WithMessage("Minimum name length is 2 characters")
                .MaximumLength(25).WithMessage("Maximum name length is 25 characters")
                .Must(name => !_context.Brands.Any(x => x.BrandName == name))
                .WithMessage("Brand name already exists.");

            RuleFor(x => x.Picture).NotNull().WithMessage("Image is required.")
                .SetValidator(new PictureDtoValidator());
        }
    }
}
