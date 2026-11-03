using Newtonsoft.Json;
using EShopper.Application.Exceptions;
using EShopper.Application.Loggers;
using EShopper.Application.UseCases;
using EShopper.Application.UseCases.DTO;
using EShopper.Domain;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace EShopper.Implementation
{
    public class UseCaseHandler
    {
        private static readonly Regex SecretFields = new Regex(
            "\"(?<key>[^\"]*(?:password|token)[^\"]*)\"\\s*:\\s*\"[^\"]*\"",
            RegexOptions.IgnoreCase);

        private IExceptionLogger _logger;
        private IUser _user;
        private IUseCaseLogger _useCaseLogger;

        public UseCaseHandler(IExceptionLogger logger, IUseCaseLogger useCaseLogger, IUser user)
        {
            _logger = logger;
            _useCaseLogger = useCaseLogger;
            _user = user;
        }

        public void HandleCommand<TRequest>(ICommand<TRequest> request, TRequest data)
        {
            try
            {
                HandleLoggingAndAuthorization(request, data);
                var sw = new Stopwatch();
                sw.Start();

                request.Execute(data);

                sw.Stop();

                Console.WriteLine("Command name: " + request.UseCaseName + ", Duration: " + sw.ElapsedMilliseconds + "ms");
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                throw;
            }
        }

        public TResponse HandleQuery<TRequest, TResponse>(IQuery<TRequest, TResponse> query, TRequest data)
        {
            try
            {
                HandleLoggingAndAuthorization(query, data);

                var sw = new Stopwatch();
                sw.Start();

                var response = query.Execute(data);
                sw.Stop();
                Console.WriteLine("Query name: " + query.UseCaseName + ", Duration: " + sw.ElapsedMilliseconds + "ms");

                return response;
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                throw;
            }
        }

        private void HandleLoggingAndAuthorization<TRequest>(IUseCase useCase, TRequest data)
        {
            var isAuthorized = _user.UseCaseIds.Contains(useCase.UseCaseId);
            var log = new UseCaseLog
            {
                User = _user.Identity,
                ExecutionDateTime = DateTime.UtcNow,
                UseCaseName = useCase.UseCaseName,
                UserId = _user.Id,
                Data = SecretFields.Replace(JsonConvert.SerializeObject(data), "\"${key}\":\"********\""),
                IsAuthorized = isAuthorized
            };

            _useCaseLogger.Log(log);

            if (!isAuthorized)
            {
                throw new ForbiddenUseCaseExecutionException(useCase.UseCaseName, _user.Identity);
            }
        }
    }
}
