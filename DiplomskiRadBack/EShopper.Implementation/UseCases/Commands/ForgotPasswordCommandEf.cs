using EShopper.Application.Email;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;

namespace EShopper.Implementation.UseCases.Commands
{
    public class ForgotPasswordCommandEf : UseCaseEf, IForgotPasswordCommand
    {
        private const string FrontendUrl = "http://localhost:4200";

        private readonly IEmail _sender;

        public int UseCaseId => 23;

        public string UseCaseName => "Forgot password";

        public string UseCaseDescription => "Send password reset link by email.";

        public ForgotPasswordCommandEf(EShopperDbContext context, IEmail sender) : base(context)
        {
            _sender = sender;
        }

        public void Execute(ForgotPasswordDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return;
            }

            var user = Context.Users.FirstOrDefault(x => x.Email == request.Email && !x.IsBanned);

            if (user == null)
            {
                return;
            }

            user.ResetPasswordToken = Guid.NewGuid().ToString("N");
            user.ResetPasswordTokenExpiry = DateTime.UtcNow.AddMinutes(30);

            Context.Users.Update(user);
            Context.SaveChanges();

            var resetLink = $"{FrontendUrl}/reset-password?token={user.ResetPasswordToken}";

            Console.WriteLine($"[DEV] Password reset requested for {user.Email}. Reset link: {resetLink}");

            try
            {
                _sender.Send(new MessageDto
                {
                    To = user.Email,
                    Title = "Reset your password",
                    Body = $"Hi {user.FirstName},<br/>Click the link below to reset your password (valid for 30 minutes):<br/><a href=\"{resetLink}\">{resetLink}</a>"
                });
            }
            catch (Exception)
            {
            }
        }
    }
}
