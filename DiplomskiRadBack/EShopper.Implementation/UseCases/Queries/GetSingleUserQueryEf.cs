using Microsoft.EntityFrameworkCore;
using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.DTO;
using EShopper.Application.UseCases.Queries;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Queries
{
    public class GetSingleUserQueryEf : UseCaseEf, IGetSingleUserQuery
    {
        private IUser _user;
        public int UseCaseId => 20;

        public string UseCaseName => "Get single user";

        public string UseCaseDescription => "Get single user with provided id";

        public GetSingleUserQueryEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public UserDtoResponse Execute(int request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2 && request != _user.Id)
            {
                throw new ForbiddenQueryException("user data");
            }

            if (!Context.Users.Any(x => x.Id == request))
            {
                throw new EntityNotExistException(nameof(User), request);
            }

            var user = new User();

            var query = Context.Users
                .Include(x => x.Info)
                .Include(x => x.Cart)
                .Include(x => x.Cart)
                .ThenInclude(x => x.Items)
                .Include(x => x.Cart)
                .ThenInclude(x => x.Items)
                .ThenInclude(x => x.Product)
                .Include(x => x.Cart)
                .ThenInclude(x => x.Items)
                .ThenInclude(x => x.Size)
                .Where(x => !x.IsBanned)
                .AsQueryable();

            if (role == 2)
            {
                query = query.Where(x => x.Id == _user.Id);

                user = query.FirstOrDefault();
            }
            else
            {
                query = query.Where(x => x.Id == request);
                user = query.FirstOrDefault();
            }

            return new UserDtoResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Username = user.Username,
                AvatarSrc = user.AvatarSrc,
                Info = new InfoDto
                {
                    Id = user.Info.Id,
                    Address = user.Info.Address,
                    City = user.Info.City,
                    PostalCode = user.Info.PostalCode,
                    Country = user.Info.Country
                }
            };
        }
    }
}
