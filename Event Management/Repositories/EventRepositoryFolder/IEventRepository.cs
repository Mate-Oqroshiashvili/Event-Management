using Event_Management.Models;
using Event_Management.Models.Dtos.EventDtos;
using Event_Management.Models.Dtos.UserDtos;

namespace Event_Management.Repositories.EventRepositoryFolder
{
    public interface IEventRepository
    {
        Task<IEnumerable<EventDto>> GetPublishedEventsAsync();
        Task<IEnumerable<EventDto>> GetDraftedEventsAsync();
        Task<IEnumerable<EventDto>> GetCompletedEventsAsync();
        Task<IEnumerable<EventDto>> GetDeletedEventsAsync();
        Task<EventDto> GetEventByIdAsync(int id);
        Task<EventDto> GetEventBySearchtermAsync(string searchTerm);
        Task<IEnumerable<EventDto>> GetEventsByOrganizerIdAsync(int organizerId);
        Task<IEnumerable<EventDto>> GetEventsByLocationIdAsync(int locationId);
        Task<EventDto> AddEventAsync(EventCreateDto eventCreateDto);
        Task<UserDto> AddSpeakerOrArtistOnEventAsync(int eventId, int userId);
        Task<string> RemoveSpeakerOrArtistFromEventAsync(int eventId, int userId);
        Task<bool> UpdateEventAsync(int id, EventUpdateDto eventUpdateDto);
        Task<bool> RescheduleEventAsync(int id, DateTime newDate);
        Task<bool> PublishTheEvent(int eventId);
        Task<bool> DeleteEventAsync(int id);
    }
}
