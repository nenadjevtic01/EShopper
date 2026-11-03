namespace EShopper.Application.Exceptions
{
    public class UnprocessableException : Exception
    {
        public UnprocessableException(string name)
            : base($"Can't set duplicate name: " + name)
        {

        }
    }
}
