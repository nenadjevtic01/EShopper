using FluentValidation;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;

namespace EShopper.Implementation.Validations
{
    public class UpdateUserInfoValidator : AbstractValidator<UpdateUserInfoDto>
    {
        private readonly EShopperDbContext _context;

        public UpdateUserInfoValidator(EShopperDbContext context)
        {
            _context = context;

            RuleFor(x => x.UserId).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("User id is required.")
                .Must(user => _context.Users.Any(x => x.Id == user)).WithMessage("User with provided id does not exist.");

            RuleFor(x => x.FirstName).Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("First name is required.")
                    .When(x => x.FirstName != null)
                    .MinimumLength(3).WithMessage("Minimum first name length is 3 characters.")
                    .MaximumLength(20).WithMessage("Maximum first name length is 20 characters.")
                    .Matches(@"^[A-Za-z ]+$").WithMessage("First name must have only letters.");

            RuleFor(x => x.LastName).Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Last name is required.")
                    .When(x => x.LastName != null)
                    .MinimumLength(3).WithMessage("Minimum last name length is 3 characters.")
                    .MaximumLength(20).WithMessage("Maximum last name length is 20 characters.")
                    .Matches(@"^[A-Za-z ]+$").WithMessage("Last name must have only letters.");

            RuleFor(x => x).Cascade(CascadeMode.Stop)
                    .Must(x => !_context.Users.Any(u => u.Email == x.Email && u.Id != x.UserId))
                    .When(x => x.Email != null)
                    .WithMessage("Email already in use.")
                    .WithName("Email");

            RuleFor(x => x.Email).Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Email is required.")
                    .MaximumLength(100).WithMessage("Maximum email length is 100 characters.")
                    .EmailAddress().WithMessage("Email invalid.")
                    .When(x => x.Email != null);

            RuleFor(x => x).Cascade(CascadeMode.Stop)
                    .Must(x => !_context.Users.Any(u => u.Username == x.UserName && u.Id != x.UserId))
                    .When(x => x.UserName != null)
                    .WithMessage("Username already in use.")
                    .WithName("UserName");

            RuleFor(x => x.UserName).Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Username is required.")
                    .When(x => x.UserName != null);

            RuleFor(x => x.Address).Cascade(CascadeMode.Stop)
                    .Matches(@"^[a-zA-Z0-9 ]{6,20}$").When(x => !string.IsNullOrEmpty(x.Address))
                    .WithMessage("Address must start with letters and be between 6 and 20 characters long.");

            RuleFor(x => x.PostalCode).Cascade(CascadeMode.Stop)
                    .Matches(@"^[0-9]{2,6}$").When(x => !string.IsNullOrEmpty(x.PostalCode)).WithMessage("Postal code must contain only numbers. Maximum of 6 characters long.");

            RuleFor(x => x.Country).Cascade(CascadeMode.Stop)
                    .MinimumLength(4).When(x => !string.IsNullOrEmpty(x.Country))
                    .WithMessage("Minimum country name length is 4 characters.")
                    .MaximumLength(60).When(x => !string.IsNullOrEmpty(x.Country))
                    .WithMessage("Maximum country name length is 60 characters.")
                    .Matches(@"^[a-zA-Z ]+$").When(x => !string.IsNullOrEmpty(x.Country))
                    .WithMessage("Country must contain only letters.");

            RuleFor(x => x.City).Cascade(CascadeMode.Stop)
                    .MinimumLength(4).When(x => !string.IsNullOrEmpty(x.City))
                    .WithMessage("Minimum city name length is 4 characters.")
                    .MaximumLength(30).When(x => !string.IsNullOrEmpty(x.City))
                    .WithMessage("Maximum city name length is 30 characters.")
                    .Matches(@"^[a-zA-Z ]+$").When(x => !string.IsNullOrEmpty(x.City))
                    .WithMessage("City must contain only letters.");
        }
    }
}
