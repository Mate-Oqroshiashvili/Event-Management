using Microsoft.Extensions.FileProviders;

namespace Event_Management.Extensions
{
    public static class StaticFileExtensions
    {
        public static IApplicationBuilder UseCustomStaticFiles(this IApplicationBuilder app)
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
                RequestPath = "/Uploads"
            });
            return app;
        }
    }
}
