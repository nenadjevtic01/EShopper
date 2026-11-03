using FluentValidation;
using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;
using EShopper.Domain;
using EShopper.Implementation.Validations;

namespace EShopper.Implementation.UseCases.Commands
{
    public class UpdateUserCommandEf : UseCaseEf, IUpdateUserInfoCommand
    {
        private readonly UpdateUserInfoValidator _validator;
        private IUser _user;
        public int UseCaseId => 6;

        public string UseCaseName => "Update user info";

        public string UseCaseDescription => "Update user information(Address, City, etc.)";

        public UpdateUserCommandEf(EShopperDbContext context, UpdateUserInfoValidator validator, IUser user) : base(context)
        {
            _validator = validator;
            _user = user;
        }

        public void Execute(UpdateUserInfoDto request)
        {
            var id = _user.Id;

            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                if (request.UserId != id)
                {
                    throw new Exception("Can't update information for another user.");
                }
            }

            _validator.ValidateAndThrow(request);

            var userToUpdate = Context.Users.Where(x => x.Id == request.UserId).FirstOrDefault();

            if (userToUpdate == null)
            {
                throw new EntityNotExistException(nameof(User), request.UserId);
            }

            if (request.FirstName != null && request.FirstName != userToUpdate.FirstName)
            {
                userToUpdate.FirstName = request.FirstName;
            }

            if (request.LastName != null && request.LastName != userToUpdate.LastName)
            {
                userToUpdate.LastName = request.LastName;
            }

            if (request.Email != null && request.Email != userToUpdate.Email)
            {
                userToUpdate.Email = request.Email;
            }

            if (request.UserName != null && request.UserName != userToUpdate.Username)
            {
                userToUpdate.Username = request.UserName;
            }

            Context.Users.Update(userToUpdate);

            var info = Context.Infos.Where(x => x.UserId == request.UserId).FirstOrDefault();

            if (info == null)
            {
                info = new Info { UserId = request.UserId };
                Context.Infos.Add(info);
            }

            if (request.Address != null)
            {
                info.Address = TextRules.NullIfBlank(request.Address);
            }

            if (request.City != null)
            {
                info.City = TextRules.NullIfBlank(request.City);
            }

            if (request.Country != null)
            {
                info.Country = TextRules.NullIfBlank(request.Country);
            }

            if (request.PostalCode != null)
            {
                info.PostalCode = TextRules.NullIfBlank(request.PostalCode);
            }

            Context.SaveChanges();
        }
    }
}
