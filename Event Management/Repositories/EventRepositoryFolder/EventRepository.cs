using AutoMapper;
using Event_Management.Data;
using Event_Management.Models;
using Event_Management.Models.Dtos.EventDtos;
using Microsoft.EntityFrameworkCore;

namespace Event_Management.Repositories.EventRepositoryFolder
{
    public class EventRepository : IEventRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public EventRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EventDto>> GetEventsAsync()
        {
            var events = await _context.Events
                .Include(x => x.Participants)
                .Include(x => x.Tickets)
                .Include(x => x.Location)
                .Include(x => x.Organizer)
                .ToListAsync();

            var eventDtos = _mapper.Map<IEnumerable<EventDto>>(events);

            return eventDtos;
        }

        public async Task<EventDto> GetEventByIdAsync(int id)
        {
            var @event = await _context.Events
                .Include(x => x.Participants)
                .Include(x => x.Tickets)
                .Include(x => x.Location)
                .Include(x => x.Organizer)
                .FirstOrDefaultAsync(x => x.Id == id);

            var eventDto = _mapper.Map<EventDto>(@event);

            return eventDto;
        }

        public async Task<EventDto> AddEventAsync(EventCreateDto eventCreateDto)
        {
            var @event = _mapper.Map<Event>(eventCreateDto);

            await _context.Events.AddAsync(@event);
            await _context.SaveChangesAsync();

            var eventDto = _mapper.Map<EventDto>(@event);

            return eventDto;
        }

        public async Task<bool> UpdateEventAsync(int id, EventUpdateDto eventUpdateDto)
        {
            var existingEvent = await _context.Events.FirstOrDefaultAsync(x => x.Id == id);
            var @event = _mapper.Map(eventUpdateDto, existingEvent);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event == null) return false;

            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
