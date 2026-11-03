using FluentValidation;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;
using EShopper.Implementation.Validations;

namespace EShopper.Implementation.UseCases.Commands
{
    public class ResetPasswordCommandEf : UseCaseEf, IResetPasswordCommand
    {
        private readonly ResetPasswordValidator _validator;

        public int UseCaseId => 24;

        public string UseCaseName => "Reset password";

        public string UseCaseDescription => "Set new password with reset token.";

        public ResetPasswordCommandEf(EShopperDbContext context, ResetPasswordValidator validator) : base(context)
        {
            _validator = validator;
        }

        public void Execute(ResetPasswordDto request)
        {
            _validator.ValidateAndThrow(request);

            var user = Context.Users.First(x =>
                x.ResetPasswordToken == request.Token &&
                x.ResetPasswordTokenExpiry != null &&
                x.ResetPasswordTokenExpiry > DateTime.UtcNow);

            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.ResetPasswordToken = null;
            user.ResetPasswordTokenExpiry = null;

            Context.Users.Update(user);
            Context.SaveChanges();
        }
    }
}
