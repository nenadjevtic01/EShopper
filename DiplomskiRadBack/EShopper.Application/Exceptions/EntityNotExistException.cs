namespace EShopper.Application.Exceptions
{
    public class EntityNotExistException : Exception
    {
        public EntityNotExistException(string type, int id)
            : base($"Entity type: " + type + " ,Id: " + id + " Status: Not found or not active.")
        {
        }
    }
}
