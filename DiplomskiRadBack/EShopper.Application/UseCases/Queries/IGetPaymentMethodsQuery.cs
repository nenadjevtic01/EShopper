using EShopper.Application.UseCases.DTO;

namespace EShopper.Application.UseCases.Queries
{
    public interface IGetPaymentMethodsQuery : IQuery<bool, IEnumerable<PaymentMethodDto>>
    {
    }
}
