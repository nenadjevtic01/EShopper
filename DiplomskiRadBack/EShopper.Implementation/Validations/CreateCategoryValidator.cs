using FluentValidation;
using EShopper.Application.Storage;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;

namespace EShopper.Implementation.Validations
{
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryDto>
    {
        private EShopperDbContext _context;

        public CreateCategoryValidator(EShopperDbContext context)
        {
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MinimumLength(3)
                .WithMessage("Minimum name length is 3 characters")
                .MaximumLength(25)
                .WithMessage("Maximum name length is 25 characters")
                .Must(CategoryNotInUse)
                .WithMessage("Category name already exists.")
                .Matches(@"^[A-Za-z ]+$")
                .WithMessage("Category must have only letters.")
                .Must(name => !CategoryFolder.IsReserved(name))
                .WithMessage("This category name is reserved.");

            RuleFor(x => x.Picture).NotNull().WithMessage("Image is required.")
                .SetValidator(new PictureDtoValidator());

            _context = context;
        }

        private bool CategoryNotInUse(string name)
        {
            return !_context.Categories.Any(x => x.CategoryName == name);
        }
    }
}
