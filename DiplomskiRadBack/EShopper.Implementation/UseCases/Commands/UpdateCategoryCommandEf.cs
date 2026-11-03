using FluentValidation;
using EShopper.Application.Storage;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;
using EShopper.Domain;
using EShopper.Implementation.Storage;
using EShopper.Implementation.Validations;

namespace EShopper.Implementation.UseCases.Commands
{
    public class UpdateCategoryCommandEf : UseCaseEf, IUpdateCategoryCommand
    {
        private readonly UpdateCategoryValidator _validator;
        private readonly IUser _user;
        private readonly AssetCleaner _cleaner;
        private readonly IAssetStorage _storage;

        public int UseCaseId => 30;

        public string UseCaseName => "Update category";

        public string UseCaseDescription => "Update category name and icon.";

        public UpdateCategoryCommandEf(
            EShopperDbContext context,
            UpdateCategoryValidator validator,
            IUser user,
            AssetCleaner cleaner,
            IAssetStorage storage) : base(context)
        {
            _validator = validator;
            _user = user;
            _cleaner = cleaner;
            _storage = storage;
        }

        public void Execute(UpdateCategoryDto request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User don't have permission to update categories.");
            }

            request.Name = CategoryFolder.CleanName(request.Name);

            _validator.ValidateAndThrow(request);

            var category = Context.Categories.First(x => x.Id == request.Id);

            var oldFolder = CategoryFolder.FromName(category.CategoryName);
            var newFolder = CategoryFolder.FromName(request.Name);
            var oldSrc = CategoryFolder.MoveSrc(category.Src ?? "", oldFolder, newFolder);

            category.CategoryName = request.Name;
            category.Src = CategoryFolder.MoveSrc(request.Picture.Src, oldFolder, newFolder);
            category.Alt = request.Picture.Alt;

            if (oldFolder != newFolder)
            {
                var marker = $"assets/img/{oldFolder}/";

                foreach (var picture in Context.Pictures.Where(x => x.Src.Contains(marker)).ToList())
                {
                    picture.Src = CategoryFolder.MoveSrc(picture.Src, oldFolder, newFolder);
                }

                _storage.MoveFolder(oldFolder, newFolder);
            }

            Context.Categories.Update(category);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                if (oldFolder != newFolder)
                {
                    _storage.MoveFolder(newFolder, oldFolder);
                }

                throw;
            }

            if (oldSrc != category.Src)
            {
                _cleaner.DeleteIfUnused(oldSrc);
            }
        }
    }
}
