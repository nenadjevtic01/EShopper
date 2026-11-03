using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.Commands;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Commands
{
    public class DeleteCartItemCommandEf : UseCaseEf, IDeleteCartItemCommand
    {
        private IUser _user;
        public int UseCaseId => 11;

        public string UseCaseName => "Delete cart item";

        public string UseCaseDescription => "Delete cart item with provided id";

        public DeleteCartItemCommandEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public void Execute(int request)
        {
            var id = _user.Id;
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 1)
            {
                throw new CustomerOnlyException("Admins can't change a cart.");
            }

            var userCart = Context.Carts.Where(x => x.UserId == id).Select(x => x.Items).AsQueryable();

            if (!userCart.Any(x => x.Any(x => x.Id == request)))
            {
                throw new Exception("Can't delete cart item from someone elses cart.");
            }

            var item = Context.CartItems.Where(x => x.Id == request).First();

            Context.CartItems.Remove(item);

            Context.SaveChanges();
        }
    }
}
