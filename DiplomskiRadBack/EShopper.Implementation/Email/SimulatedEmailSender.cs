using EShopper.Application.Email;

namespace EShopper.Implementation.Email
{
    public class SimulatedEmailSender : IEmail
    {
        public void Send(MessageDto dto)
        {
            Console.WriteLine($"[EMAIL SIMULATION] To: {dto.To} | Subject: {dto.Title}");
        }
    }
}
