using EShopper.Application.UseCases;
using EShopper.Application.UseCases.DTO;
using EShopper.DataAccess;
using EShopper.Domain;

namespace EShopper.Implementation.Loggers
{
    public class UseCaseLogger : IUseCaseLogger
    {
        private EShopperDbContext _context;

        public UseCaseLogger(EShopperDbContext context)
        {
            _context = context;
        }

        public void Log(UseCaseLog log)
        {
            var auditLog = new AuditLog
            {
                UserId = log.UserId,
                Name = log.User,
                UseCaseName = log.UseCaseName,
                ExecutionDate = log.ExecutionDateTime,
                Data = log.Data,
                IsAuthorized = log.IsAuthorized
            };

            _context.AuditLogs.Add(auditLog);
            _context.SaveChanges();
        }
    }
}
