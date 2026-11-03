using EShopper.Application.Loggers;

namespace EShopper.Implementation.Loggers
{
    public class ConsoleLogger : IExceptionLogger
    {
        public void Log(Exception ex)
        {
            Console.WriteLine("Exception occured at: " + DateTime.UtcNow.AddHours(2));
            Console.WriteLine("Exception message: " + ex.Message);
        }
    }
}
