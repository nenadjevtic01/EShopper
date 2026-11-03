using FluentValidation;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;
using EShopper.Domain;
using EShopper.Implementation.Validations;

namespace EShopper.Implementation.UseCases.Commands
{
    public class CreateBrandCommandEf : UseCaseEf, ICreateBrandCommand
    {
        private readonly CreateBrandValidator _validator;
        private readonly IUser _user;

        public int UseCaseId => 27;

        public string UseCaseName => "Create brand";

        public string UseCaseDescription => "Create new brand.";

        public CreateBrandCommandEf(EShopperDbContext context, CreateBrandValidator validator, IUser user) : base(context)
        {
            _validator = validator;
            _user = user;
        }

        public void Execute(CreateBrandDto request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User don't have permission to add new brand.");
            }

            request.Name = request.Name?.Trim();

            _validator.ValidateAndThrow(request);

            var brand = new Brand
            {
                BrandName = request.Name,
                Src = request.Picture.Src,
                Alt = request.Picture.Alt
            };

            Context.Brands.Add(brand);
            Context.SaveChanges();
        }
    }
}
