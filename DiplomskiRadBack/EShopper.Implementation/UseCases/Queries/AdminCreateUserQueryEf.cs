using FluentValidation;
using EShopper.Application.Email;
using EShopper.Application.UseCases.DTO;
using EShopper.Application.UseCases.Queries;
using EShopper.DataAccess;
using EShopper.Domain;
using EShopper.Implementation.Validations;

namespace EShopper.Implementation.UseCases.Queries
{
    public class AdminCreateUserQueryEf : UseCaseEf, IAdminCreateUserQuery
    {
        private const string FrontendUrl = "http://localhost:4200";

        private readonly AdminCreateUserValidator _validator;
        private readonly IEmail _sender;
        private readonly IUser _user;

        public int UseCaseId => 25;

        public string UseCaseName => "Admin create user";

        public string UseCaseDescription => "Create user and email link to set password.";

        public AdminCreateUserQueryEf(
            EShopperDbContext context,
            AdminCreateUserValidator validator,
            IEmail sender,
            IUser user) : base(context)
        {
            _validator = validator;
            _sender = sender;
            _user = user;
        }

        public AdminCreateUserResultDto Execute(AdminCreateUserDto request)
        {
            var role = Context.Users.Find(_user.Id)?.Role;

            if (role == 2)
            {
                throw new Exception("User don't have permission to create users.");
            }

            _validator.ValidateAndThrow(request);

            var info = new Info
            {
                Address = TextRules.NullIfBlank(request.Address),
                City = TextRules.NullIfBlank(request.City),
                PostalCode = TextRules.NullIfBlank(request.PostalCode),
                Country = TextRules.NullIfBlank(request.Country)
            };

            var cart = new Cart();
            var newRole = request.Role ?? 2;

            var useCases = new List<UserUseCase>
            {
                new UserUseCase { UseCaseId = 1 },
                new UserUseCase { UseCaseId = 5 },
                new UserUseCase { UseCaseId = 6 },
                new UserUseCase { UseCaseId = 7 },
                new UserUseCase { UseCaseId = 8 },
                new UserUseCase { UseCaseId = 9 },
                new UserUseCase { UseCaseId = 10 },
                new UserUseCase { UseCaseId = 11 },
                new UserUseCase { UseCaseId = 14 },
                new UserUseCase { UseCaseId = 15 },
                new UserUseCase { UseCaseId = 16 },
                new UserUseCase { UseCaseId = 20 },
                new UserUseCase { UseCaseId = 26 },
                new UserUseCase { UseCaseId = 33 },
                new UserUseCase { UseCaseId = 34 },
                new UserUseCase { UseCaseId = 38 },
                new UserUseCase { UseCaseId = 44 },
                new UserUseCase { UseCaseId = 45 },
                new UserUseCase { UseCaseId = 50 },
                new UserUseCase { UseCaseId = 55 },
                new UserUseCase { UseCaseId = 56 },
                new UserUseCase { UseCaseId = 57 },
                new UserUseCase { UseCaseId = 58 },
                new UserUseCase { UseCaseId = 62 },
            };

            if (newRole == 1)
            {
                useCases = Context.UseCases
                    .Where(x => !RolePermissions.GuestOnlyUseCases.Contains(x.Id) && !RolePermissions.CustomerOnlyUseCases.Contains(x.Id))
                    .Select(x => new UserUseCase { UseCaseId = x.Id }).ToList();
            }

            var resetToken = Guid.NewGuid().ToString("N");

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Username = request.UserName,
                Password = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()),
                IsBanned = false,
                Role = newRole,
                Info = info,
                Cart = cart,
                UserUseCases = useCases,
                ResetPasswordToken = resetToken,
                ResetPasswordTokenExpiry = DateTime.UtcNow.AddDays(3)
            };

            Context.Infos.Add(info);
            Context.Carts.Add(cart);
            Context.Users.Add(user);
            Context.SaveChanges();

            var resetLink = $"{FrontendUrl}/reset-password?token={resetToken}";
            var emailSent = true;

            Console.WriteLine($"[DEV] Admin invited {user.Email}. Set-password link: {resetLink}");

            try
            {
                _sender.Send(new MessageDto
                {
                    To = user.Email,
                    Title = "You've been invited",
                    Body = $"Hi {user.FirstName},<br/>An account was created for you. Click the link below to set your password (valid for 3 days):<br/><a href=\"{resetLink}\">{resetLink}</a>"
                });
            }
            catch (Exception)
            {
                emailSent = false;
            }

            return new AdminCreateUserResultDto
            {
                UserId = user.Id,
                Email = user.Email,
                ResetLink = resetLink,
                EmailSent = emailSent
            };
        }
    }
}
