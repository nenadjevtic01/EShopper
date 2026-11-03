using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.Commands;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Commands
{
    public class DeleteProductCommandEf : UseCaseEf, IDeleteProductCommand
    {
        private IUser _user;
        public int UseCaseId => 13;

        public string UseCaseName => "Delete product";

        public string UseCaseDescription => "Toggle a product's active status (soft delete).";

        public DeleteProductCommandEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public void Execute(int request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User don't have permission to delete product.");
            }

            var product = Context.Products.Where(x => x.Id == request).FirstOrDefault();

            if (product == null)
            {
                throw new EntityNotExistException(nameof(Product), request);
            }

            if (!product.IsActive)
            {
                if (!Context.Categories.Any(x => x.Id == product.CategoryId && x.IsActive))
                {
                    throw new UseCaseConflictException("Can't reactivate a product while its category is inactive.");
                }

                if (!Context.Brands.Any(x => x.Id == product.BrandId && x.IsActive))
                {
                    throw new UseCaseConflictException("Can't reactivate a product while its brand is inactive.");
                }
            }

            product.IsActive = !product.IsActive;

            Context.Products.Update(product);
            Context.SaveChanges();
        }
    }
}
