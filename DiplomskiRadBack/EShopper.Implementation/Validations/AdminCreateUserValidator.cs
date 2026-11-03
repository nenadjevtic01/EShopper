using FluentValidation;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;

namespace EShopper.Implementation.Validations
{
    public class AdminCreateUserValidator : AbstractValidator<AdminCreateUserDto>
    {
        private readonly EShopperDbContext _context;

        public AdminCreateUserValidator(EShopperDbContext context)
        {
            _context = context;

            RuleFor(x => x.FirstName).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("First name is required.")
                .MinimumLength(3).WithMessage("Minimum first name length is 3 characters.")
                .MaximumLength(20).WithMessage("Maximum first name length is 20 characters.")
                .Matches(@"^[A-Za-z ]+$").WithMessage("First name must have only letters.");

            RuleFor(x => x.LastName).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Last name is required.")
                .MinimumLength(3).WithMessage("Minimum last name length is 3 characters.")
                .MaximumLength(20).WithMessage("Maximum last name length is 20 characters.")
                .Matches(@"^[A-Za-z ]+$").WithMessage("Last name must have only letters.");

            RuleFor(x => x.Email).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Email is required.")
                .MaximumLength(100).WithMessage("Maximum email length is 100 characters.")
                .EmailAddress().WithMessage("Email invalid.")
                .Must(email => !_context.Users.Any(u => u.Email == email && u.IsActive)).WithMessage("Email already in use.");

            RuleFor(x => x.Role)
                .Must(role => role == null || role == 1 || role == 2).WithMessage("Role must be 1 (administrator) or 2 (user).");

            RuleFor(x => x.UserName).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(6).WithMessage("Minimum username length is 6 characters.")
                .MaximumLength(20).WithMessage("Maximum username length is 20 characters.")
                .Matches(@"^(?=[a-z0-9._]{6,20}$)(?!.*[_.]{2})[^_.].*[^_.]$")
                .WithMessage("Username must have between 6 and 20 characters. Can't start or end with . or _. Can't have .. or ._ or __ inside.")
                .Must(username => !_context.Users.Any(u => u.Username == username && u.IsActive)).WithMessage("Username already in use.");

            RuleFor(x => x.Address).Cascade(CascadeMode.Stop)
                    .Matches(@"^[a-zA-Z0-9 ]{6,20}$")
                    .When(x => !string.IsNullOrEmpty(x.Address))
                    .WithMessage("Address must start with letters and be between 6 and 20 characters long.");

            RuleFor(x => x.PostalCode).Cascade(CascadeMode.Stop)
                    .Matches(@"^[0-9]{2,6}$")
                    .When(x => !string.IsNullOrEmpty(x.PostalCode))
                    .WithMessage("Postal code must contain only numbers. Maximum of 6 characters long.");

            RuleFor(x => x.Country).Cascade(CascadeMode.Stop)
                    .MinimumLength(4).When(x => !string.IsNullOrEmpty(x.Country))
                    .WithMessage("Minimum country name length is 4 characters.")
                    .MaximumLength(60).When(x => !string.IsNullOrEmpty(x.Country))
                    .WithMessage("Maximum country name length is 60 characters.")
                    .Matches(@"^[a-zA-Z ]+$").When(x => !string.IsNullOrEmpty(x.Country))
                    .WithMessage("Country must contain only letters.");

            RuleFor(x => x.City).Cascade(CascadeMode.Stop)
                    .Matches(@"^[a-zA-Z ]+$").When(x => !string.IsNullOrEmpty(x.City))
                    .WithMessage("City must contain only letters.");
        }
    }
}
