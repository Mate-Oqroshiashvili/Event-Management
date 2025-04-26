using Event_Management.Data;
using Microsoft.EntityFrameworkCore;

namespace Event_Management.Repositories.PromoCodeRepositoryFolder.Background_Services
{
    public class PromoCodeResetService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory; // Factory for creating service scopes

        public PromoCodeResetService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        /// <summary>
        /// Executes the background service to reset promo codes.
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Wait until the application is fully started
            while (!stoppingToken.IsCancellationRequested)
            {
                // Create a scope to access the database context
                using (var scope = _scopeFactory.CreateScope())
                {
                    // Get the DataContext from the service provider
                    var context = scope.ServiceProvider.GetRequiredService<DataContext>();

                    var usersToUpdate = await context.Users
                        .Where(u => !u.PromoCodeIsClaimable &&
                           (
                               (u.LastPromoClaimedAt.HasValue && u.LastPromoClaimedAt.Value.AddDays(3) <= DateTime.UtcNow)
                               || !u.LastPromoClaimedAt.HasValue
                           ))
                        .ToListAsync(); // Fetch users who are eligible for promo code reset

                    // Update the PromoCodeIsClaimable property for eligible users
                    foreach (var user in usersToUpdate)
                    {
                        user.PromoCodeIsClaimable = true;
                    }

                    // Save changes to the database
                    if (usersToUpdate.Count > 0)
                        await context.SaveChangesAsync();
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // Check every 1h
            }
        }
    }
}
