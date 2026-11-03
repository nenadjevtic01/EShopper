using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.Commands;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Commands
{
    public class DeleteReceiptCommandEf : UseCaseEf, IDeleteReceiptCommand
    {
        private IUser _user;
        public int UseCaseId => 17;

        public string UseCaseName => "Delete receipt";

        public string UseCaseDescription => "Toggle a receipt's active status (soft delete).";

        public DeleteReceiptCommandEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public void Execute(int request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("Only an admin can delete an order.");
            }

            var receipt = Context.Receipts.Where(x => x.Id == request).FirstOrDefault();

            if (receipt == null)
            {
                throw new EntityNotExistException(nameof(Receipt), request);
            }

            receipt.IsActive = !receipt.IsActive;

            Context.Receipts.Update(receipt);
            Context.SaveChanges();
        }
    }
}
