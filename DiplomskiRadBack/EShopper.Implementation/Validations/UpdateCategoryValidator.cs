using FluentValidation;
using EShopper.Application.Storage;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;

namespace EShopper.Implementation.Validations
{
    public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryDto>
    {
        private EShopperDbContext _context;

        public UpdateCategoryValidator(EShopperDbContext context)
        {
            _context = context;

            RuleFor(x => x.Id).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Category id is required.")
                .Must(id => _context.Categories.Any(x => x.Id == id)).WithMessage("Category with provided id does not exist.");

            RuleFor(x => x.Name).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(3).WithMessage("Minimum name length is 3 characters")
                .MaximumLength(25).WithMessage("Maximum name length is 25 characters")
                .Matches(@"^[A-Za-z ]+$").WithMessage("Category must have only letters.")
                .Must(name => !CategoryFolder.IsReserved(name)).WithMessage("This category name is reserved.")
                .Must((dto, name) => !_context.Categories.Any(x => x.CategoryName == name && x.Id != dto.Id))
                .WithMessage("Category name already exists.");

            RuleFor(x => x.Picture).SetValidator(new PictureDtoValidator());
        }
    }
}
