using FluentValidation;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;
using EShopper.Domain;
using EShopper.Implementation.Storage;
using EShopper.Implementation.Validations;

namespace EShopper.Implementation.UseCases.Commands
{
    public class UpdateBrandCommandEf : UseCaseEf, IUpdateBrandCommand
    {
        private readonly UpdateBrandValidator _validator;
        private readonly IUser _user;
        private readonly AssetCleaner _cleaner;

        public int UseCaseId => 28;

        public string UseCaseName => "Update brand";

        public string UseCaseDescription => "Update brand name and logo.";

        public UpdateBrandCommandEf(
            EShopperDbContext context,
            UpdateBrandValidator validator,
            IUser user,
            AssetCleaner cleaner) : base(context)
        {
            _validator = validator;
            _user = user;
            _cleaner = cleaner;
        }

        public void Execute(UpdateBrandDto request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User don't have permission to update brands.");
            }

            request.Name = request.Name?.Trim();

            _validator.ValidateAndThrow(request);

            var brand = Context.Brands.First(x => x.Id == request.Id);

            var oldSrc = brand.Src ?? "";

            brand.BrandName = request.Name;
            brand.Src = request.Picture.Src;
            brand.Alt = request.Picture.Alt;

            Context.Brands.Update(brand);
            Context.SaveChanges();

            if (oldSrc != brand.Src)
            {
                _cleaner.DeleteIfUnused(oldSrc);
            }
        }
    }
}
