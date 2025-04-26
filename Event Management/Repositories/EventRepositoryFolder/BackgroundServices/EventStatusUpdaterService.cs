using Event_Management.Data;
using Event_Management.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Event_Management.Repositories.EventRepositoryFolder.BackgroundServices
{
    public class EventStatusUpdaterService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory; // Factory for creating service scopes
        private readonly ILogger<EventStatusUpdaterService> _logger; // Logger for logging information and errors

        public EventStatusUpdaterService(IServiceScopeFactory serviceScopeFactory, ILogger<EventStatusUpdaterService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        /// <summary>
        /// Executes the background service to update event statuses.
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("EventStatusUpdaterService is running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Create a new scope for the service
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                        var utcNow = DateTime.UtcNow;
                        var eventsToUpdate = await dbContext.Events
                            .Where(e => e.EndDate < utcNow && e.Status == EventStatus.PUBLISHED && e.Status != EventStatus.COMPLETED)
                            .Include(e => e.Location)
                            .ToListAsync(stoppingToken);

                        // Check if there are any events to update
                        if (eventsToUpdate.Count != 0)
                        {
                            // Update the status of the events to COMPLETED
                            foreach (var ev in eventsToUpdate)
                            {
                                ev.Status = EventStatus.COMPLETED;
                                if (ev.Location != null)
                                {
                                    ev.Location.RemainingCapacity += ev.Capacity;
                                    ev.Location.AvailableStaff += ev.BookedStaff;
                                    ev.Location.BookedStaff -= ev.BookedStaff;
                                }
                            }

                            // Save the changes to the database
                            await dbContext.SaveChangesAsync(stoppingToken);
                            _logger.LogInformation($"Updated {eventsToUpdate.Count} events to COMPLETED status.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error updating event statuses: {ex.Message}");
                }

                // Wait for 10 minutes before checking again (adjust as needed)
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }
    }
}
