using Event_Management.Exceptions;
using Event_Management.Models.Error;
using System.Net;

namespace Event_Management.Extensions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next; // Delegate to the next middleware in the pipeline
        private readonly ILogger<ExceptionMiddleware> _logger; // Logger for logging exceptions

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context) // Method to invoke the middleware
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something Went Wrong: {ex}");

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex) // Method to handle the exception
        {
            context.Response.ContentType = "application/json";

            var statusCode = ex switch
            {
                NotFoundException => HttpStatusCode.NotFound,
                BadRequestException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError,
            };

            context.Response.StatusCode = (int)statusCode;

            var response = new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = ex.Message,
                RequestId = context.TraceIdentifier,
                StackTrace = ex.StackTrace ?? string.Empty,
                Details = ex.InnerException?.Message ?? "No inner exception"
            };

            await context.Response.WriteAsync(response.ToString());
        }
    }
}
