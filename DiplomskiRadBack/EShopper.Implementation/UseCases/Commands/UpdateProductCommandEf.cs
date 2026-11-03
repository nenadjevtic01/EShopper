using FluentValidation;
using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;
using EShopper.Domain;
using EShopper.Implementation.Storage;
using EShopper.Implementation.Validations;

namespace EShopper.Implementation.UseCases.Commands
{
    public class UpdateProductCommandEf : UseCaseEf, IUpdateProductCommand
    {
        private UpdateProductValidator _validator;
        private IUser _user;
        private AssetCleaner _cleaner;
        public int UseCaseId => 21;

        public string UseCaseName => "Update product";

        public string UseCaseDescription => "Update existing product details and sizes.";

        public UpdateProductCommandEf(
            EShopperDbContext context,
            UpdateProductValidator validator,
            IUser user,
            AssetCleaner cleaner) : base(context)
        {
            _validator = validator;
            _user = user;
            _cleaner = cleaner;
        }

        public void Execute(UpdateProductDto request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new System.Exception("User don't have permission to update product.");
            }

            _validator.ValidateAndThrow(request);

            var product = Context.Products.Where(x => x.Id == request.ProductId).FirstOrDefault();

            if (product == null)
            {
                throw new EntityNotExistException(nameof(Product), request.ProductId);
            }

            product.ProductName = request.ProductName;
            product.CategoryId = request.CategoryId;
            product.BrandId = request.BrandId;
            product.GenderId = request.GenderId;
            product.InStock = request.Sizes.Any(x => x.Quantity > 0);
            product.Material = request.Material;
            product.CountryId = request.CountryId;

            var oldPictures = Context.Pictures.Where(x => x.ProductId == request.ProductId).ToList();
            var oldSources = oldPictures.Select(x => x.Src).ToList();
            var oldSizes = Context.ProductSizes.Where(x => x.ProductId == request.ProductId).ToList();

            Context.Pictures.RemoveRange(oldPictures);
            Context.ProductSizes.RemoveRange(oldSizes);

            var newPictures = request.Pictures.Select(x => new Picture
            {
                Src = x.Src,
                Alt = x.Alt,
                Product = product
            });

            var newSizes = request.Sizes.Select(x => new ProductSize
            {
                Product = product,
                SizeId = x.SizeId,
                Quantity = x.Quantity
            });

            Context.Products.Update(product);
            Context.Pictures.AddRange(newPictures);
            Context.ProductSizes.AddRange(newSizes);

            Context.SaveChanges();

            foreach (var oldSrc in oldSources.Except(request.Pictures.Select(x => x.Src)))
            {
                _cleaner.DeleteIfUnused(oldSrc);
            }
        }
    }
}
