using FluentValidation;
using EShopper.Application.UseCases.DTO;

namespace EShopper.Implementation.Validations
{
    public class CreateContactMessageValidator : AbstractValidator<CreateContactMessageDto>
    {
        public CreateContactMessageValidator()
        {
            RuleFor(x => x.Name).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(3).WithMessage("Minimum name length is 3.")
                .MaximumLength(100).WithMessage("Maximum name length is 100.");

            RuleFor(x => x.Email).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email format is incorrect.");

            RuleFor(x => x.Subject).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Subject is required.")
                .MinimumLength(3).WithMessage("Minimum subject length is 3.")
                .MaximumLength(150).WithMessage("Maximum subject length is 150.");

            RuleFor(x => x.Message).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Message is required.")
                .MinimumLength(3).WithMessage("Minimum message length is 3.")
                .MaximumLength(1000).WithMessage("Maximum message length is 1000.");
        }
    }
}
