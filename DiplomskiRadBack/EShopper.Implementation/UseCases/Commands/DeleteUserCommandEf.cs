using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.Commands;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Commands
{
    public class DeleteUserCommandEf : UseCaseEf, IDeleteUserCommand
    {
        private IUser _user;
        public int UseCaseId => 22;

        public string UseCaseName => "Delete user";

        public string UseCaseDescription => "Ban user account (soft delete).";

        public DeleteUserCommandEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public void Execute(int request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User don't have permission to delete user.");
            }

            if (request == _user.Id)
            {
                throw new Exception("Can't delete your own account.");
            }

            var user = Context.Users.Where(x => x.Id == request).FirstOrDefault();

            if (user == null)
            {
                throw new EntityNotExistException(nameof(User), request);
            }

            user.IsBanned = !user.IsBanned;

            Context.Users.Update(user);
            Context.SaveChanges();
        }
    }
}
