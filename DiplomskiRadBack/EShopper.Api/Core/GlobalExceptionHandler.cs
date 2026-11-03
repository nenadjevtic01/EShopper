using FluentValidation;
using EShopper.Application.Exceptions;
using EShopper.Application.Loggers;

namespace EShopper.Api.Core
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly IExceptionLogger _logger;

        public GlobalExceptionHandler(RequestDelegate next, IExceptionLogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (System.Exception ex)
            {
                _logger.Log(ex);

                httpContext.Response.ContentType = "application/json";
                object response = null;
                var statusCode = StatusCodes.Status500InternalServerError;

                if (ex is ForbiddenUseCaseExecutionException || ex is ForbiddenQueryException)
                {
                    statusCode = StatusCodes.Status403Forbidden;
                }

                if (ex is CustomerOnlyException customerOnlyEx)
                {
                    statusCode = StatusCodes.Status403Forbidden;
                    response = new { message = customerOnlyEx.Message };
                }

                if (ex is EntityNotExistException)
                {
                    statusCode = StatusCodes.Status404NotFound;
                }

                if (ex is ItemAlreadyExistException)
                {
                    statusCode = StatusCodes.Status409Conflict;
                }

                if (ex is ValidationException e)
                {
                    statusCode = StatusCodes.Status422UnprocessableEntity;
                    response = e.Errors.Any()
                        ? (object)new
                        {
                            errors = e.Errors.Select(x => new
                            {
                                property = x.PropertyName,
                                error = x.ErrorMessage
                            })
                        }
                        : new { message = e.Message };
                }

                if (ex is UseCaseConflictException conflictEx)
                {
                    statusCode = StatusCodes.Status409Conflict;
                    response = new { message = conflictEx.Message };
                }

                if (statusCode == StatusCodes.Status500InternalServerError && response == null)
                {
                    response = new
                    {
                        message = ex.GetType() == typeof(System.Exception)
                            ? ex.Message
                            : "An unexpected error occurred. Please try again later."
                    };
                }

                httpContext.Response.StatusCode = statusCode;

                if (response != null)
                {
                    await httpContext.Response.WriteAsJsonAsync(response);
                }
            }
        }
    }
}
