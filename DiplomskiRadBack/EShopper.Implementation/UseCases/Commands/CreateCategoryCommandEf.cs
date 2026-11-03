using FluentValidation;
using EShopper.Application.Storage;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;
using EShopper.Domain;
using EShopper.Implementation.Validations;

namespace EShopper.Implementation.UseCases.Commands
{
    public class CreateCategoryCommandEf : UseCaseEf, ICreateCategoryCommand
    {
        private CreateCategoryValidator _validator;
        private IUser _user;
        private readonly IAssetStorage _storage;
        public int UseCaseId => 2;
        public string UseCaseName => "Create category";

        public string UseCaseDescription => "Creating new category";

        public CreateCategoryCommandEf(EShopperDbContext context, CreateCategoryValidator validator, IUser user, IAssetStorage storage) : base(context)
        {
            _validator = validator;
            _user = user;
            _storage = storage;
        }

        public void Execute(CreateCategoryDto request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User don't have permission to add new category.");
            }

            request.Name = CategoryFolder.CleanName(request.Name);

            _validator.ValidateAndThrow(request);

            var category = new Category
            {
                CategoryName = request.Name,
                Src = request.Picture.Src,
                Alt = request.Picture.Alt
            };

            Context.Categories.Add(category);

            Context.SaveChanges();

            _storage.EnsureFolder(CategoryFolder.FromName(category.CategoryName));
        }
    }
}
