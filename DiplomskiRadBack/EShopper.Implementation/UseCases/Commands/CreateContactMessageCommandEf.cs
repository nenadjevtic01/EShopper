using FluentValidation;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;
using EShopper.Domain;
using EShopper.Implementation.Validations;

namespace EShopper.Implementation.UseCases.Commands
{
    public class CreateContactMessageCommandEf : UseCaseEf, ICreateContactMessageCommand
    {
        private readonly CreateContactMessageValidator _validator;

        public int UseCaseId => 34;

        public string UseCaseName => "Create contact message";

        public string UseCaseDescription => "Send message from contact form.";

        public CreateContactMessageCommandEf(EShopperDbContext context, CreateContactMessageValidator validator) : base(context)
        {
            _validator = validator;
        }

        public void Execute(CreateContactMessageDto request)
        {
            _validator.ValidateAndThrow(request);

            var message = new ContactMessage
            {
                Name = request.Name,
                Email = request.Email,
                Subject = request.Subject,
                Message = request.Message
            };

            Context.ContactMessages.Add(message);
            Context.SaveChanges();
        }
    }
}
