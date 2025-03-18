using Event_Management.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddServices(builder.Configuration);
builder.Services.AddAuthServices(builder.Configuration);

var app = builder.Build();

app.UseCustomStaticFiles();

// Use CORS middleware
app.UseCors("AllowAngularApp");

app.UseCustomSwagger(app.Environment);

app.UseHttpsRedirection();
app.UseRouting();

app.UseCustomMiddleware();

app.MapControllers();

app.Run();
