using Event_Management.Data;
using Event_Management.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Event_Management.Repositories.EventRepositoryFolder.BackgroundServices
{
    public class EventStatusUpdaterService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<EventStatusUpdaterService> _logger;

        public EventStatusUpdaterService(IServiceScopeFactory serviceScopeFactory, ILogger<EventStatusUpdaterService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("EventStatusUpdaterService is running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                        var utcNow = DateTime.UtcNow;
                        var eventsToUpdate = await dbContext.Events
                            .Where(e => e.EndDate < utcNow && e.Status == EventStatus.PUBLISHED && e.Status != EventStatus.COMPLETED)
                            .Include(e => e.Location)
                            .ToListAsync(stoppingToken);

                        if (eventsToUpdate.Count != 0)
                        {
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
