using FluentValidation;
using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;
using EShopper.Domain;
using EShopper.Implementation.Validations;

namespace EShopper.Implementation.UseCases.Commands
{
    public class CreateProductCommandEf : UseCaseEf, ICreateProductCommand
    {
        private CreateProductValidator _validator;
        private IUser _user;
        public int UseCaseId => 12;

        public string UseCaseName => "Create product.";

        public string UseCaseDescription => "Create new product.";

        public CreateProductCommandEf(EShopperDbContext context, CreateProductValidator validator, IUser user) : base(context)
        {
            _validator = validator;
            _user = user;
        }

        public void Execute(CreateProductDto request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User don't have permission to add new product.");
            }

            _validator.ValidateAndThrow(request);

            var product = Context.Products
                .Where(x =>
                    x.CategoryId == request.CategoryId
                    && x.BrandId == request.BrandId
                    && x.GenderId == request.GenderId
                    && x.ProductName == request.ProductName)
                .FirstOrDefault();

            if (product != null)
            {
                throw new ItemAlreadyExistException(nameof(product));
            }

            product = new Product
            {
                ProductName = request.ProductName,
                GenderId = request.GenderId,
                CategoryId = request.CategoryId,
                BrandId = request.BrandId,
                InStock = request.Sizes.Any(x => x.Quantity > 0),
                Material = request.Material,
                CountryId = request.CountryId,
            };

            var pictures = request.Pictures.Select(x => new Picture
            {
                Src = x.Src,
                Alt = x.Alt,
                Product = product
            });

            var sizes = request.Sizes.Select(x => new ProductSize
            {
                Product = product,
                SizeId = x.SizeId,
                Quantity = x.Quantity
            });

            var price = new Price
            {
                ActiveFrom = request.Price.ActiveFrom,
                NewPrice = request.Price.Price,
                Product = product
            };

            Context.Products.Add(product);
            Context.Pictures.AddRange(pictures);
            Context.ProductSizes.AddRange(sizes);
            Context.Prices.Add(price);

            Context.SaveChanges();
        }
    }
}
