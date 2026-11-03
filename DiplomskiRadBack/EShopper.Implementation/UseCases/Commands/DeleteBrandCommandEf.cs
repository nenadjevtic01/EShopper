using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.Commands;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Commands
{
    public class DeleteBrandCommandEf : UseCaseEf, IDeleteBrandCommand
    {
        private readonly IUser _user;

        public int UseCaseId => 29;

        public string UseCaseName => "Delete brand";

        public string UseCaseDescription => "Toggle brand active status.";

        public DeleteBrandCommandEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public void Execute(int request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User don't have permission to delete brands.");
            }

            var brand = Context.Brands.Where(x => x.Id == request).FirstOrDefault();

            if (brand == null)
            {
                throw new EntityNotExistException(nameof(Brand), request);
            }

            if (brand.IsActive && Context.Products.Any(x => x.BrandId == request && x.IsActive))
            {
                throw new UseCaseConflictException("Can't delete a brand that still has active products assigned to it.");
            }

            brand.IsActive = !brand.IsActive;

            Context.Brands.Update(brand);
            Context.SaveChanges();
        }
    }
}
