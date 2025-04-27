using Event_Management.Models;
using Event_Management.Models.Dtos.EventDtos;
using Event_Management.Models.Dtos.UserDtos;

namespace Event_Management.Repositories.EventRepositoryFolder
{
    public interface IEventRepository
    {
        Task<IEnumerable<EventDto>> GetMostPopularPublishedEventsAsync(); // Retrieves the most popular published events
        Task<IEnumerable<EventDto>> GetMostRecentPublishedEventsAsync(); // Retrieves the most recent published events
        Task<IEnumerable<EventDto>> GetPublishedEventsAsync(); // Retrieves all published events
        Task<IEnumerable<EventDto>> GetDraftedEventsAsync(); // Retrieves all drafted events
        Task<IEnumerable<EventDto>> GetCompletedEventsAsync(); // Retrieves all completed events
        Task<IEnumerable<EventDto>> GetDeletedEventsAsync(); // Retrieves all deleted events
        Task<EventDto> GetEventByIdAsync(int id); // Retrieves an event by its ID
        Task<IEnumerable<EventDto>> GetEventBySearchtermAsync(string searchTerm); // Retrieves events by search term
        Task<IEnumerable<EventDto>> GetEventsByOrganizerIdAsync(int organizerId); // Retrieves events by organizer ID
        Task<IEnumerable<EventDto>> GetEventsByLocationIdAsync(int locationId); // Retrieves events by location ID
        Task<EventAnalyticsDto> GetEventAnalyticsAsync(int organizerId, int eventId); // Retrieves event analytics by organizer ID and event ID
        Task<EventDto> AddEventAsync(EventCreateDto eventCreateDto); // Adds a new event
        Task<UserDto> AddSpeakerOrArtistOnEventAsync(int eventId, int userId); // Adds a speaker or artist to an event
        Task<string> RemoveSpeakerOrArtistFromEventAsync(int eventId, int userId); // Removes a speaker or artist from an event
        Task<bool> UpdateEventAsync(int id, EventUpdateDto eventUpdateDto); // Updates an existing event
        Task<bool> RescheduleEventAsync(int id, RescheduleEventDto rescheduleEventDto); // Reschedules an event
        Task<bool> PublishTheEvent(int eventId); // Publishes an event
        Task<bool> DeleteEventAsync(int id); // Deletes an event by its ID
    }
}
