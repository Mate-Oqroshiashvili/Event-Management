using Event_Management.Models;
using Event_Management.Models.Dtos.EventDtos;
using Event_Management.Models.Dtos.UserDtos;

namespace Event_Management.Repositories.EventRepositoryFolder
{
    public interface IEventRepository
    {
        Task<IEnumerable<EventDto>> GetEventsAsync();
        Task<EventDto> GetEventByIdAsync(int id);
        Task<EventDto> AddEventAsync(EventCreateDto eventCreateDto);
        Task<bool> UpdateEventAsync(int id, EventUpdateDto eventUpdateDto);
        Task<bool> RescheduleEventAsync(int id, DateTime newDate);
        Task<bool> DeleteEventAsync(int id);
    }
}
