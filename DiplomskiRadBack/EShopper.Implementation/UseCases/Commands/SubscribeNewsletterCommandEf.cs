using System.Text.RegularExpressions;
using FluentValidation;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Commands
{
    public class SubscribeNewsletterCommandEf : UseCaseEf, ISubscribeNewsletterCommand
    {
        public int UseCaseId => 38;

        public string UseCaseName => "Subscribe to newsletter";

        public string UseCaseDescription => "Subscribe email to newsletter.";

        public SubscribeNewsletterCommandEf(EShopperDbContext context) : base(context)
        {
        }

        public void Execute(SubscribeNewsletterDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || !Regex.IsMatch(request.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                throw new ValidationException("A valid e-mail address is required.");
            }

            if (request.Email.Length > 100)
            {
                throw new ValidationException("Maximum email length is 100 characters.");
            }

            var existing = Context.NewsletterSubscribers.FirstOrDefault(x => x.Email == request.Email);

            if (existing != null)
            {
                if (!existing.IsActive)
                {
                    existing.IsActive = true;
                    Context.NewsletterSubscribers.Update(existing);
                    Context.SaveChanges();
                }

                return;
            }

            Context.NewsletterSubscribers.Add(new NewsletterSubscriber { Email = request.Email });
            Context.SaveChanges();
        }
    }
}
