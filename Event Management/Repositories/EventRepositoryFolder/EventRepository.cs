using AutoMapper;
using Event_Management.Data;
using Event_Management.Exceptions;
using Event_Management.Models;
using Event_Management.Models.Dtos.EventDtos;
using Event_Management.Models.Enums;
using Event_Management.Repositories.CodeRepositoryFolder;
using Event_Management.Repositories.ImageRepositoryFolder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Event_Management.Repositories.EventRepositoryFolder
{
    public class EventRepository : IEventRepository
    {
        private readonly DataContext _context;
        private readonly ICodeRepository _codeRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IMapper _mapper;

        public EventRepository(DataContext context, ICodeRepository codeRepository, IImageRepository imageRepository, IMapper mapper)
        {
            _context = context;
            _codeRepository = codeRepository;
            _imageRepository = imageRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EventDto>> GetEventsAsync()
        {
            var events = await _context.Events
                    .Where(x => x.Status != EventStatus.DELETED && x.Status != EventStatus.DRAFT && x.Status != EventStatus.COMPLETED)
                    .Include(x => x.Participants)
                        .ThenInclude(x => x.Ticket)
                    .Include(x => x.Participants)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Tickets)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Tickets)
                        .ThenInclude(x => x.Participant)
                    .Include(x => x.Tickets)
                        .ThenInclude(x => x.Purchase)
                    .Include(e => e.Location)
                        .ThenInclude(l => l.Events)
                    .Include(x => x.Location)
                        .ThenInclude(x => x.Organizers)
                    .Include(x => x.Organizer)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Organizer)
                        .ThenInclude(x => x.Locations)
                    .Include(x => x.Organizer)
                        .ThenInclude(x => x.Events)
                    .Include(x => x.Location)
                    .Include(x => x.SpeakersAndArtists)
                    .Include(x => x.PromoCodes)
                    .Include(x => x.Reviews)
                    .Include(x => x.Comments)
                    .ToListAsync();

            var eventDtos = _mapper.Map<IEnumerable<EventDto>>(events);

            return eventDtos;
        }

        public async Task<EventDto> GetEventByIdAsync(int id)
        {
            var @event = await _context.Events
                    .Include(x => x.Participants)
                        .ThenInclude(x => x.Ticket)
                    .Include(x => x.Participants)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Tickets)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Tickets)
                        .ThenInclude(x => x.Participant)
                    .Include(x => x.Tickets)
                        .ThenInclude(x => x.Purchase)
                    .Include(e => e.Location)
                        .ThenInclude(l => l.Events)
                    .Include(x => x.Location)
                        .ThenInclude(x => x.Organizers)
                    .Include(x => x.Organizer)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Organizer)
                        .ThenInclude(x => x.Locations)
                    .Include(x => x.Organizer)
                        .ThenInclude(x => x.Events)
                    .Include(x => x.Location)
                    .Include(x => x.SpeakersAndArtists)
                    .Include(x => x.PromoCodes)
                    .Include(x => x.Reviews)
                    .Include(x => x.Comments)
                    .FirstOrDefaultAsync(x => x.Id == id && x.Status != EventStatus.DELETED);

            var eventDto = _mapper.Map<EventDto>(@event);

            return eventDto;
        }

        public async Task<EventDto> GetEventBySearchtermAsync(string searchTerm)
        {
            var @event = await _context.Events
                    .Include(x => x.Participants)
                        .ThenInclude(x => x.Ticket)
                    .Include(x => x.Participants)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Tickets)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Tickets)
                        .ThenInclude(x => x.Participant)
                    .Include(x => x.Tickets)
                        .ThenInclude(x => x.Purchase)
                    .Include(e => e.Location)
                        .ThenInclude(l => l.Events)
                    .Include(x => x.Location)
                        .ThenInclude(x => x.Organizers)
                    .Include(x => x.Organizer)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Organizer)
                        .ThenInclude(x => x.Locations)
                    .Include(x => x.Organizer)
                        .ThenInclude(x => x.Events)
                    .Include(x => x.Location)
                    .Include(x => x.SpeakersAndArtists)
                    .Include(x => x.PromoCodes)
                    .Include(x => x.Reviews)
                    .Include(x => x.Comments)
                    .FirstOrDefaultAsync(x => x.Title == searchTerm || x.Description.Contains(searchTerm) && x.Status != EventStatus.DELETED && x.Status != EventStatus.DRAFT);

            var eventDto = _mapper.Map<EventDto>(@event);

            return eventDto;
        }

        public async Task<IEnumerable<EventDto>> GetEventsByOrganizerIdAsync(int organizerId)
        {
            var events = await _context.Events
                    .Where(e => e.OrganizerId == organizerId)
                    .Include(x => x.Participants)
                        .ThenInclude(x => x.Ticket)
                    .Include(x => x.Participants)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Tickets)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Tickets)
                        .ThenInclude(x => x.Participant)
                    .Include(x => x.Tickets)
                        .ThenInclude(x => x.Purchase)
                    .Include(e => e.Location)
                        .ThenInclude(l => l.Events)
                    .Include(x => x.Location)
                        .ThenInclude(x => x.Organizers)
                    .Include(x => x.Organizer)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Organizer)
                        .ThenInclude(x => x.Locations)
                    .Include(x => x.Organizer)
                        .ThenInclude(x => x.Events)
                    .Include(x => x.Location)
                    .Include(x => x.SpeakersAndArtists)
                    .Include(x => x.PromoCodes)
                    .Include(x => x.Reviews)
                    .Include(x => x.Comments)
                    .ToListAsync();

            var eventDtos = _mapper.Map<IEnumerable<EventDto>>(events);

            return eventDtos;
        }

        public async Task<EventDto> AddEventAsync(EventCreateDto eventCreateDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var @event = _mapper.Map<Event>(eventCreateDto);
                @event.Capacity = eventCreateDto.Capacity;

                @event.Organizer = await _context.Organizers
                    .Include(x => x.Events)
                    .Include(x => x.Locations)
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == eventCreateDto.OrganizerId)
                    ?? throw new NotFoundException("Event organizer not found!");

                @event.Location = await _context.Locations
                    .Include(x => x.Events)
                    .Include(x => x.Organizers)
                    .FirstOrDefaultAsync(x => x.Id == @event.LocationId)
                    ?? throw new NotFoundException("Event location not found!");

                if (!@event.Organizer.IsVerified)
                    throw new BadRequestException("Organizer is not verified!");

                try
                {
                    @event.Images = await Task.WhenAll(eventCreateDto.Images
                        .Select(async image => await _imageRepository.GenerateImageSource(image)));
                }
                catch (Exception ex)
                {
                    throw new BadRequestException(ex.Message, ex.InnerException);
                }

                @event.OrganizerId = eventCreateDto.OrganizerId;

                if (!@event.Location.Organizers.Any(o => o.Id == @event.Organizer.Id))
                {
                    @event.Location.Organizers.Add(@event.Organizer);
                }

                if (!@event.Organizer.Locations.Any(l => l.Id == @event.Location.Id))
                {
                    @event.Organizer.Locations.Add(@event.Location);
                }

                if (!@event.Organizer.Events.Any(l => l.Id == @event.Id))
                {
                    @event.Organizer.Events.Add(@event);
                }

                if (!@event.Location.Events.Any(e => e.Id == @event.Id))
                {
                    @event.Location.Events.Add(@event);
                }

                if (@event.Capacity <= 0)
                    throw new BadRequestException("Event capacity must be greater than zero!");

                if (@event.Capacity > @event.Location.RemainingCapacity)
                    throw new BadRequestException("Event capacity exceeds location’s maximum capacity!");

                @event.Location.RemainingCapacity -= @event.Capacity;
                var ratio = @event.Capacity == 0 ? 1 : (double)@event.Location.MaxCapacity / @event.Capacity;
                @event.BookedStaff = (int)Math.Floor(@event.Location.AvailableStaff / ratio);
                @event.Location.AvailableStaff -= @event.BookedStaff;
                @event.Location.BookedStaff += @event.BookedStaff;

                await _context.Events.AddAsync(@event); 
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var savedEvent = await GetEventByIdAsync(@event.Id);

                return savedEvent;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        public async Task<bool> UpdateEventAsync(int id, EventUpdateDto eventUpdateDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingEvent = await _context.Events
                    .Include(x => x.Location)
                    .FirstOrDefaultAsync(x => x.Id == id)
                    ?? throw new NotFoundException("Event does not exist!");

                existingEvent.Organizer = await _context.Organizers
                    .Include(x => x.Events)
                    .Include(x => x.Locations)
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == existingEvent.OrganizerId)
                    ?? throw new NotFoundException("Organizer not found!");

                if (!existingEvent.Organizer.IsVerified)
                    throw new BadRequestException("Organizer is not verified!");

                existingEvent.Location = await _context.Locations
                    .Include(x => x.Events)
                    .Include(x => x.Organizers)
                    .FirstOrDefaultAsync(x => x.Id == eventUpdateDto.LocationId)
                    ?? throw new NotFoundException("Event location not found!");

                existingEvent.Images = await Task.WhenAll(eventUpdateDto.Images
                    .Select(async image => await _imageRepository.GenerateImageSource(image)));

                _mapper.Map(eventUpdateDto, existingEvent);

                if (eventUpdateDto.Capacity != null)
                {
                    int capacityDifference = eventUpdateDto.Capacity.Value - existingEvent.Capacity;

                    if (eventUpdateDto.Capacity > existingEvent.Location.MaxCapacity)
                        throw new BadRequestException("Event capacity exceeds location’s maximum capacity!");

                    existingEvent.Location.RemainingCapacity -= capacityDifference;

                    var ratio = eventUpdateDto.Capacity == 0 ? 1 : (double)existingEvent.Location.MaxCapacity / eventUpdateDto.Capacity.Value;
                    int newBookedStaff = (int)Math.Floor(existingEvent.Location.AvailableStaff / ratio);

                    int bookedStaffDifference = newBookedStaff - existingEvent.BookedStaff;

                    existingEvent.Location.AvailableStaff -= bookedStaffDifference;
                    existingEvent.Location.BookedStaff += bookedStaffDifference;
                    existingEvent.BookedStaff = newBookedStaff;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        public async Task<bool> RescheduleEventAsync(int id, DateTime newDate)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var @event = await _context.Events
                    .Include(x => x.Participants)
                        .ThenInclude(x => x.Ticket)
                    .Include(x => x.Participants)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Tickets)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Tickets)
                        .ThenInclude(x => x.Participant)
                    .Include(x => x.Tickets)
                        .ThenInclude(x => x.Purchase)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (@event == null) return false;

                @event.Organizer = await _context.Organizers
                    .Include(x => x.Events)
                    .Include(x => x.Locations)
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == @event.OrganizerId)
                    ?? throw new NotFoundException("Organizer not found!");

                if (!@event.Organizer.IsVerified)
                    throw new BadRequestException("Organizer is not verified!");

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
                    .Include(x => x.Participants)
                        .ThenInclude(x => x.Ticket)
                    .Include(x => x.Participants)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Tickets)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Tickets)
                        .ThenInclude(x => x.Participant)
                    .Include(x => x.Tickets)
                        .ThenInclude(x => x.Purchase)
                    .Include(x => x.PromoCodes)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (@event == null) return false;

                @event.Organizer = await _context.Organizers
                    .Include(x => x.Events)
                    .Include(x => x.Locations)
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == @event.OrganizerId)
                    ?? throw new NotFoundException("Organizer not found!");

                @event.Location = await _context.Locations
                    .Include(x => x.Events)
                    .Include(x => x.Organizers)
                    .FirstOrDefaultAsync(x => x.Id == @event.LocationId)
                    ?? throw new NotFoundException("Event location not found!");

                if (!@event.Organizer.IsVerified)
                    throw new BadRequestException("Organizer is not verified!");

                // Notify participants about event cancellation
                foreach (var participant in @event.Participants)
                {
                    await _codeRepository.SendEventCancellationNotification(participant.User.Email, @event.Title);
                }

                @event.Status = EventStatus.DELETED;

                _context.Events.Update(@event);

                // Handle purchases: Refund if necessary
                foreach (var ticket in @event.Tickets)
                {
                    ticket.Status = TicketStatus.CANCELED;

                    if (ticket.Purchase != null)
                    {
                        ticket.Purchase.Status = PurchaseStatus.REFUNDED;
                        ticket.Purchase.User.Balance += ticket.Purchase.TotalAmount;
                    }
                }
                _context.Tickets.UpdateRange(@event.Tickets);
                _context.Purchases.UpdateRange(@event.Tickets.Where(t => t.Purchase != null).Select(t => t.Purchase!));

                @event.Organizer.Events.Remove(@event);

                // Restore location capacity and staff
                @event.Location.RemainingCapacity += @event.Capacity;
                @event.Location.AvailableStaff += @event.BookedStaff;
                @event.Location.BookedStaff -= @event.BookedStaff;

                _context.Locations.Update(@event.Location);
                @event.Location.Events.Remove(@event);

                // Remove participants
                _context.Participants.RemoveRange(@event.Participants);

                foreach (var promoCode in @event.PromoCodes)
                {
                    _context.PromoCodes.Remove(promoCode);
                }

                //// Delete the event or leave it as it is for the soft deletion
                //_context.Events.Remove(@event);

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
