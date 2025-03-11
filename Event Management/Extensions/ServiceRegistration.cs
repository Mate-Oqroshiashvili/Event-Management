using Event_Management.Data;
using Event_Management.Helpers;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Event_Management.Extensions
{
    public static class ServiceRegistration
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddAutoMapper(typeof(EventMappingProfile));
            services.AddAutoMapper(typeof(LocationMappingProfile));
            services.AddAutoMapper(typeof(OrganizerMappingProfile));
            services.AddAutoMapper(typeof(ParticipantMappingProfile));
            services.AddAutoMapper(typeof(PurchaseMappingProfile));
            services.AddAutoMapper(typeof(TicketMappingProfile));
            services.AddAutoMapper(typeof(UserMappingProfile));

            //fluent validator
            services.AddControllers()
                .AddFluentValidation(v =>
                {
                    v.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
                });

            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // CORS setup
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            // dependency service registration
            services.AddDependencyServices(configuration);

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });
        }
    }
}
