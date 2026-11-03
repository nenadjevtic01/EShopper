using FluentValidation;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;

namespace EShopper.Implementation.Validations
{
    public class UpdateProductValidator : AbstractValidator<UpdateProductDto>
    {
        private EShopperDbContext _context;

        public UpdateProductValidator(EShopperDbContext context)
        {
            _context = context;

            RuleFor(x => x.ProductId).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Product id is required.")
                .Must(id => _context.Products.Any(x => x.Id == id)).WithMessage("Product with provided id does not exist.");

            RuleFor(x => x.ProductName).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Product name is required.")
                .MinimumLength(5).WithMessage("Minimum product name length is 5 characters.")
                .MaximumLength(25).WithMessage("Maximum product name length is 25 characters.")
                .Matches(@"^[A-Za-z0-9 ]+$").WithMessage("Product must have only letters and numbers.");

            RuleFor(x => x.CategoryId).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Category id is required.")
                .Must(id => _context.Categories.Any(x => x.Id == id)).WithMessage("Category with provided id does not exist.");

            RuleFor(x => x.BrandId).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Brand id is required.")
                .Must(id => _context.Brands.Any(x => x.Id == id)).WithMessage("Brand with provided id does not exist.");

            RuleFor(x => x.GenderId).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Gender id is required.")
                .Must(id => _context.Genders.Any(x => x.Id == id)).WithMessage("Gender with provided id does not exist.");

            RuleFor(x => x.Material).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Material is required.");

            RuleFor(x => x.CountryId).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Country is required.")
                .Must(id => _context.Countries.Any(x => x.Id == id)).WithMessage("Country with provided id does not exist.");

            RuleFor(x => x.Sizes).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Minimum number of sizes is 1.")
                .Must(size =>
                {
                    if (size == null)
                    {
                        return true;
                    }

                    return size.Select(s => s.SizeId).Distinct().Count() == size.Count();
                }).WithMessage("Duplicate size id are not allowed.").DependentRules(() =>
                {
                    RuleForEach(x => x.Sizes)
                        .Must(x => _context.Sizes.Any(y => y.Id == x.SizeId))
                        .WithMessage("Size with provided id does not exist");
                    RuleForEach(x => x.Sizes).Must(x => x.Quantity >= 0).WithMessage("Quantity can't be negative.");
                });

            RuleFor(x => x.Pictures).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("Pictures are required")
                .Must(isUnique).WithMessage("Duplicate picture src are not allowed.");

            RuleForEach(x => x.Pictures).SetValidator(new PictureDtoValidator());
        }

        public bool isUnique(IEnumerable<PictureDto> pictures)
        {
            var srcs = new HashSet<string>();

            foreach (var picture in pictures)
            {
                if (srcs.Contains(picture.Src))
                    return false;
                srcs.Add(picture.Src);
            }

            return true;
        }
    }
}
