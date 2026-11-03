using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.Commands;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Commands
{
    public class DeleteCategoryCommandEf : UseCaseEf, IDeleteCategoryCommand
    {
        private readonly IUser _user;

        public int UseCaseId => 31;

        public string UseCaseName => "Delete category";

        public string UseCaseDescription => "Toggle category active status.";

        public DeleteCategoryCommandEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public void Execute(int request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User don't have permission to delete categories.");
            }

            var category = Context.Categories.Where(x => x.Id == request).FirstOrDefault();

            if (category == null)
            {
                throw new EntityNotExistException(nameof(Category), request);
            }

            if (category.IsActive && Context.Products.Any(x => x.CategoryId == request && x.IsActive))
            {
                throw new UseCaseConflictException("Can't delete a category that still has active products assigned to it.");
            }

            category.IsActive = !category.IsActive;

            Context.Categories.Update(category);
            Context.SaveChanges();
        }
    }
}
