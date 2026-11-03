using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.Commands;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Commands
{
    public class DeleteContactMessageCommandEf : UseCaseEf, IDeleteContactMessageCommand
    {
        private readonly IUser _user;

        public int UseCaseId => 35;

        public string UseCaseName => "Delete contact message";

        public string UseCaseDescription => "Toggle contact message active status.";

        public DeleteContactMessageCommandEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public void Execute(int request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User don't have permission to delete contact messages.");
            }

            var message = Context.ContactMessages.Where(x => x.Id == request).FirstOrDefault();

            if (message == null)
            {
                throw new EntityNotExistException(nameof(ContactMessage), request);
            }

            message.IsActive = !message.IsActive;
            Context.ContactMessages.Update(message);
            Context.SaveChanges();
        }
    }
}
