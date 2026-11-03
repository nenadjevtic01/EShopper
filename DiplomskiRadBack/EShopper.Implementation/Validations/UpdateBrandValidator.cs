using FluentValidation;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;

namespace EShopper.Implementation.Validations
{
    public class UpdateBrandValidator : AbstractValidator<UpdateBrandDto>
    {
        private EShopperDbContext _context;

        public UpdateBrandValidator(EShopperDbContext context)
        {
            _context = context;

            RuleFor(x => x.Id).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Brand id is required.")
                .Must(id => _context.Brands.Any(x => x.Id == id)).WithMessage("Brand with provided id does not exist.");

            RuleFor(x => x.Name).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(2).WithMessage("Minimum name length is 2 characters")
                .MaximumLength(25).WithMessage("Maximum name length is 25 characters")
                .Must((dto, name) => !_context.Brands.Any(x => x.BrandName == name && x.Id != dto.Id))
                .WithMessage("Brand name already exists.");

            RuleFor(x => x.Picture).SetValidator(new PictureDtoValidator());
        }
    }
}
