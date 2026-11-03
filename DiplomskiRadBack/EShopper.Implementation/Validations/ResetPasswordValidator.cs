using FluentValidation;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;

namespace EShopper.Implementation.Validations
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordDto>
    {
        private readonly EShopperDbContext _context;

        public ResetPasswordValidator(EShopperDbContext context)
        {
            _context = context;

            RuleFor(x => x.Token).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Reset token is required.")
                .Must(token => _context.Users.Any(u =>
                    u.ResetPasswordToken == token &&
                    u.ResetPasswordTokenExpiry != null &&
                    u.ResetPasswordTokenExpiry > DateTime.UtcNow))
                .WithMessage("Reset link is invalid or has expired. Please request a new one.");

            RuleFor(x => x.NewPassword).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Password is required.")
                .Matches(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$")
                .WithMessage("Minimum eight characters, at least one letter and one number.");
        }
    }
}
