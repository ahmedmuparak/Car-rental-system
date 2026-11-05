using CarRental.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.API.Middleware
{
    public class GlobalExceptionMiddleware : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionMiddleware> logger;

        public GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger)
        {
            this.logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var statusCode = exception switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                BadRequestException => StatusCodes.Status400BadRequest,
                ForbiddenException => StatusCodes.Status403Forbidden,
                ConflictException => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };
            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                logger.LogError(exception, "An unexpected error occurred.");
            }
            else
            {
                logger.LogWarning(exception, "Request failed with status code {StatusCode}", statusCode);
            }


            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = statusCode switch
                {
                    404 => "Not Found",
                    400 => "Bad Request",
                    409 => "Conflict",
                    _ => "Internal Server Error"
                },
                Detail = statusCode == 500
        ? "An unexpected error occurred."
        : exception.Message,
                Instance = httpContext.Request.Path
            };

            httpContext.Response.StatusCode = statusCode;

            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsJsonAsync(
                problemDetails,
                cancellationToken);

            return true;
        }
    }
}
