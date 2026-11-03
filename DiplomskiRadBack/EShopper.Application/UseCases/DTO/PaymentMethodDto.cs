namespace EShopper.Application.UseCases.DTO
{
    public class PaymentMethodDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreatePaymentMethodDto
    {
        public string Name { get; set; }
    }

    public class UpdatePaymentMethodDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
