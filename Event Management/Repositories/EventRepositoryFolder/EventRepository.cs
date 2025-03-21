using AutoMapper;
using Event_Management.Data;
using Event_Management.Models;
using Event_Management.Models.Dtos.EventDtos;
using Event_Management.Models.Enums;
using Event_Management.Repositories.CodeRepositoryFolder;
using Microsoft.EntityFrameworkCore;

namespace Event_Management.Repositories.EventRepositoryFolder
{
    public class EventRepository : IEventRepository
    {
        private readonly DataContext _context;
        private readonly ICodeRepository _codeRepository;
        private readonly IMapper _mapper;

        public EventRepository(DataContext context, ICodeRepository codeRepository, IMapper mapper)
        {
            _context = context;
            _codeRepository = codeRepository;
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

        public async Task<IEnumerable<EventDto>> GetEventsByOrganizerIdAsync(int organizerId)
        {
            var events = await _context.Events
                .Where(e => e.OrganizerId == organizerId)
                .Include(x => x.Participants)
                .Include(x => x.Tickets)
                .Include(x => x.Location)
                .Include(x => x.Organizer)
                .ToListAsync();

            var eventDtos = _mapper.Map<IEnumerable<EventDto>>(events);

            return eventDtos;
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

        public async Task<bool> RescheduleEventAsync(int id, DateTime newDate)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var @event = await _context.Events
                    .Include(e => e.Tickets)
                    .Include(e => e.Participants)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (@event == null) return false;

                // Update the event date
                var duration = @event.EndDate - @event.StartDate;
                @event.StartDate = newDate;
                @event.EndDate = newDate + duration;
                _context.Events.Update(@event);

                // Notify participants (pseudo-code, replace with real email service)
                foreach (var participant in @event.Participants)
                {
                    await _codeRepository.SendEventRescheduleNotification(participant.User.Email, @event.Title, newDate);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var @event = await _context.Events
                    .Include(e => e.Tickets)
                    .Include(e => e.Participants)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (@event == null) return false;

                // Notify participants about event cancellation
                foreach (var participant in @event.Participants)
                {
                    await _codeRepository.SendEventCancellationNotification(participant.User.Email, @event.Title);
                }

                // Handle tickets: Mark as canceled or remove
                foreach (var ticket in @event.Tickets)
                {
                    ticket.Status = TicketStatus.CANCELED;
                    _context.Tickets.Update(ticket);
                }

                // Handle purchases: Refund if necessary
                foreach (Ticket ticket in @event.Tickets)
                {
                    if (ticket.Purchase != null)
                    {
                        ticket.Purchase.Status = PurchaseStatus.REFUNDED; // Mark as refunded (actual refund logic may vary)
                        ticket.Purchase.User.Balance += ticket.Purchase.TotalAmount;
                        _context.Purchases.Update(ticket.Purchase);
                    }
                }

                // Remove participants
                _context.Participants.RemoveRange(@event.Participants);

                // Delete the event
                _context.Events.Remove(@event);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
