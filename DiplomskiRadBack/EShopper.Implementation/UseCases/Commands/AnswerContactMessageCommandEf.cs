using FluentValidation;
using EShopper.Application.Exceptions;
using EShopper.Application.UseCases.Commands;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases.Commands
{
    public class AnswerContactMessageCommandEf : UseCaseEf, IAnswerContactMessageCommand
    {
        private readonly IUser _user;

        public int UseCaseId => 37;

        public string UseCaseName => "Answer contact message";

        public string UseCaseDescription => "Reply to contact message.";

        public AnswerContactMessageCommandEf(EShopperDbContext context, IUser user) : base(context)
        {
            _user = user;
        }

        public void Execute(AnswerContactMessageDto request)
        {
            var role = Context.Users.Where(x => x.Id == _user.Id).Select(x => x.Role).First();

            if (role == 2)
            {
                throw new Exception("User don't have permission to answer contact messages.");
            }

            if (string.IsNullOrWhiteSpace(request.Answer))
            {
                throw new ValidationException("Answer is required.");
            }

            var message = Context.ContactMessages.Where(x => x.Id == request.Id).FirstOrDefault();

            if (message == null)
            {
                throw new EntityNotExistException(nameof(ContactMessage), request.Id);
            }

            message.Answer = request.Answer;
            message.IsAnswered = true;
            message.AnsweredAt = DateTime.UtcNow;
            Context.ContactMessages.Update(message);
            Context.SaveChanges();
        }
    }
}
