using EShopper.Application.UseCases.DTO;
using EShopper.Application.UseCases.Queries;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Queries
{
    public class GetPaymentMethodsQueryEf : UseCaseEf, IGetPaymentMethodsQuery
    {
        public int UseCaseId => 33;

        public string UseCaseName => "Get payment methods";

        public string UseCaseDescription => "Get list of payment methods.";

        private readonly IUser _user;

        public GetPaymentMethodsQueryEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public IEnumerable<PaymentMethodDto> Execute(bool request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => (int?)x.Role).FirstOrDefault();
            var includeInactive = request && role == 1;

            return Context.PaymentMethods.Where(x => includeInactive || x.IsActive).OrderBy(x => x.Name).Select(x => new PaymentMethodDto
            {
                Id = x.Id,
                Name = x.Name,
                IsActive = x.IsActive
            }).ToList();
        }
    }
}
