using Event_Management.Extensions;
using Event_Management.Web_Sockets;

var builder = WebApplication.CreateBuilder(args); // Create a new web application builder

// Add services to the container.
builder.Services.AddServices(builder.Configuration);
builder.Services.AddAuthServices(builder.Configuration);

var app = builder.Build(); // Build the application

app.UseCustomStaticFiles(); // Use static files middleware

// Use CORS middleware
app.UseCors("AllowAngular");

app.UseCustomSwagger(app.Environment); // Use Swagger middleware

app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS
app.UseRouting(); // Use routing middleware

app.UseCustomMiddleware(); // Use custom middleware for error handling + Authentication/Authorization

app.MapControllers(); // Map controllers to routes

app.MapHub<CommentHub>("/commentHub"); // Map SignalR hub for comments
app.MapHub<CommentHub>("/userHub"); // Map SignalR hub for user notifications

app.Run(); // Run the application
