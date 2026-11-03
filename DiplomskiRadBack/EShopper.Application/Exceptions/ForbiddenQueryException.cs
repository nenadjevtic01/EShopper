namespace EShopper.Application.Exceptions
{
    public class ForbiddenQueryException : Exception
    {
        public ForbiddenQueryException() : base("User has tried to see someone else receipt.")
        {
        }

        public ForbiddenQueryException(string resource) : base($"User has tried to see someone else's {resource}.")
        {
        }
    }
}
