namespace EShopper.Application.Exceptions
{
    public class ItemAlreadyExistException : Exception
    {
        public ItemAlreadyExistException(string type) : base($"{type} already exist.")
        {
        }
    }
}
