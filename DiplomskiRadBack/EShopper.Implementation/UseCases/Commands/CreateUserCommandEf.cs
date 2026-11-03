using FluentValidation;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;
using EShopper.Domain;
using EShopper.Implementation.Validations;

namespace EShopper.Implementation.UseCases.Commands
{
    public class CreateUserCommandEf : UseCaseEf, ICreateUserCommand
    {
        private CreateUserValidator _validator;

        public int UseCaseId => 4;

        public string UseCaseName => "Create new user";

        public string UseCaseDescription => "Create new user description";

        public CreateUserCommandEf(EShopperDbContext context, CreateUserValidator validator) : base(context)
        {
            _validator = validator;
        }

        public void Execute(CreateUserDto request)
        {

            _validator.ValidateAndThrow(request);

            var info = new Info
            {
                Address = TextRules.NullIfBlank(request.Address),
                City = TextRules.NullIfBlank(request.City),
                PostalCode = TextRules.NullIfBlank(request.PostalCode),
                Country = TextRules.NullIfBlank(request.Country)
            };

            var cart = new Cart();

            var useCases = new List<UserUseCase>
            {
                new UserUseCase
                {
                    UseCaseId = 1
                },
                new UserUseCase
                {
                    UseCaseId = 5
                },
                new UserUseCase
                {
                    UseCaseId = 6
                },
                new UserUseCase
                {
                    UseCaseId = 7
                },
                new UserUseCase
                {
                    UseCaseId = 8
                },
                new UserUseCase
                {
                    UseCaseId = 9
                },
                new UserUseCase
                {
                    UseCaseId = 10
                },
                new UserUseCase
                {
                    UseCaseId = 11
                },
                new UserUseCase
                {
                    UseCaseId = 14
                },
                new UserUseCase
                {
                    UseCaseId = 15
                },
                new UserUseCase
                {
                    UseCaseId = 16
                },
                new UserUseCase
                {
                    UseCaseId = 20
                },
                new UserUseCase
                {
                    UseCaseId = 26
                },
                new UserUseCase
                {
                    UseCaseId = 33
                },
                new UserUseCase
                {
                    UseCaseId = 34
                },
                new UserUseCase
                {
                    UseCaseId = 38
                },
                new UserUseCase
                {
                    UseCaseId = 44
                },
                new UserUseCase
                {
                    UseCaseId = 45
                },
                new UserUseCase
                {
                    UseCaseId = 50
                },
                new UserUseCase
                {
                    UseCaseId = 55
                },
                new UserUseCase
                {
                    UseCaseId = 56
                },
                new UserUseCase
                {
                    UseCaseId = 57
                },
                new UserUseCase
                {
                    UseCaseId = 58
                },
                new UserUseCase
                {
                    UseCaseId = 62
                }
            };

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Username = request.UserName,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                IsBanned = false,
                Role = 2,
                Info = info,
                Cart = cart,
                UserUseCases = useCases
            };

            Context.Infos.Add(info);
            Context.Carts.Add(cart);
            Context.Users.Add(user);

            Context.SaveChanges();
        }
    }
}
