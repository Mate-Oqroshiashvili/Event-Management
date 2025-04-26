namespace Event_Management.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>(); // Custom middleware for handling exceptions
            app.UseAuthentication(); // Ensure authentication is used before authorization
            app.UseAuthorization(); // Ensure authorization is used after authentication
            return app;
        }
    }
}
