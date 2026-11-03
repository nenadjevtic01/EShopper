namespace EShopper.Application.Email
{
    public interface IEmail
    {
        void Send(MessageDto message);
    }
}
