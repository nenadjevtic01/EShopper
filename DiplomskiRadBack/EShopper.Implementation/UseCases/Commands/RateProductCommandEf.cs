using FluentValidation;
using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Commands
{
    public class RateProductCommandEf : UseCaseEf, IRateProductCommand
    {
        private readonly IUser _user;

        public int UseCaseId => 45;

        public string UseCaseName => "Rate product";

        public string UseCaseDescription => "Rate a product the user bought.";

        public RateProductCommandEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public void Execute(RateProductDto request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 1)
            {
                throw new CustomerOnlyException("Admins can't rate products.");
            }

            if (request.Rating < 1 || request.Rating > 5)
            {
                throw new ValidationException("Rating must be between 1 and 5.");
            }

            if (request.Rating * 2 != Math.Floor(request.Rating * 2))
            {
                throw new ValidationException("Rating must be in increments of 0.5.");
            }

            var hasPurchased = Context.ReceiptsItems.Any(x => x.ProductId == request.ProductId && x.Receipt.UserId == _user.Id);

            if (!hasPurchased)
            {
                throw new Exception("You can only rate products you have purchased.");
            }

            var alreadyRated = Context.ProductRatings.Any(x => x.ProductId == request.ProductId && x.UserId == _user.Id);

            if (alreadyRated)
            {
                throw new Exception("You have already rated this product.");
            }

            Context.ProductRatings.Add(new ProductRating
            {
                ProductId = request.ProductId,
                UserId = _user.Id,
                Rating = request.Rating,
                Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim()
            });

            Context.SaveChanges();
        }
    }
}
