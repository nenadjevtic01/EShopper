namespace EShopper.Application.Exceptions
{
    public class CustomerOnlyException : Exception
    {
        public CustomerOnlyException(string message) : base(message)
        {
        }
    }
}
