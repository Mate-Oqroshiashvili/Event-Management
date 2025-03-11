using Event_Management.Repositories.AuthRepositoryFolder;
using Event_Management.Repositories.CodeRepositoryFolder;
using Event_Management.Repositories.EventRepositoryFolder;
using Event_Management.Repositories.JwtRepositoryFolder;
using Event_Management.Repositories.LocationRepositoryFolder;
using Event_Management.Repositories.OrganizerRepositoryFolder;
using Event_Management.Repositories.ParticipantRepositoryFolder;
using Event_Management.Repositories.PurchaseRepositoryFolder;
using Event_Management.Repositories.TicketRepositoryFolder;
using Event_Management.Repositories.UserRepositoryFolder;

namespace Event_Management.Extensions
{
    public static class DependencyRegistration
    {
        public static void AddDependencyServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<ICodeRepository, CodeRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<IOrganizerRepository, OrganizerRepository>();
            services.AddScoped<IParticipantRepository, ParticipantRepository>();
            services.AddScoped<IPurchaseRepository, PurchaseRepository>();
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
        }
    }
}
