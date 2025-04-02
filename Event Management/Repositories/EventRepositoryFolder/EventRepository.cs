using AutoMapper;
using Event_Management.Data;
using Event_Management.Exceptions;
using Event_Management.Models;
using Event_Management.Models.Dtos.EventDtos;
using Event_Management.Models.Dtos.UserDtos;
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

        public async Task<IEnumerable<EventDto>> GetPublishedEventsAsync()
        {
            try
            {
                var events = await _context.Events
                    .Where(x => x.Status == EventStatus.PUBLISHED)
                    .Include(x => x.Participants)
                    .Include(x => x.Tickets)
                    .Include(e => e.Location)
                    .Include(x => x.Organizer)
                    .Include(x => x.SpeakersAndArtists)
                    .Include(x => x.PromoCodes)
                    .Include(x => x.Reviews)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Comments)
                        .ThenInclude(x => x.User)
                    .ToListAsync() ?? throw new NotFoundException("Events not found!");

                var eventDtos = _mapper.Map<IEnumerable<EventDto>>(events);

                return eventDtos;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        public async Task<IEnumerable<EventDto>> GetDraftedEventsAsync()
        {
            try
            {
                var events = await _context.Events
                    .Where(x => x.Status == EventStatus.DRAFT)
                    .Include(x => x.Participants)
                    .Include(x => x.Tickets)
                    .Include(e => e.Location)
                    .Include(x => x.Organizer)
                    .Include(x => x.SpeakersAndArtists)
                    .Include(x => x.PromoCodes)
                    .Include(x => x.Reviews)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Comments)
                        .ThenInclude(x => x.User)
                    .ToListAsync() ?? throw new NotFoundException("Events not found!");

                var eventDtos = _mapper.Map<IEnumerable<EventDto>>(events);

                return eventDtos;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        public async Task<IEnumerable<EventDto>> GetCompletedEventsAsync()
        {
            try
            {
                var events = await _context.Events
                    .Where(x => x.Status == EventStatus.COMPLETED)
                    .Include(x => x.Participants)
                    .Include(x => x.Tickets)
                    .Include(e => e.Location)
                    .Include(x => x.Organizer)
                    .Include(x => x.SpeakersAndArtists)
                    .Include(x => x.PromoCodes)
                    .Include(x => x.Reviews)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Comments)
                        .ThenInclude(x => x.User)
                    .ToListAsync() ?? throw new NotFoundException("Events not found!");

                var eventDtos = _mapper.Map<IEnumerable<EventDto>>(events);

                return eventDtos;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        public async Task<IEnumerable<EventDto>> GetDeletedEventsAsync()
        {
            try
            {
                var events = await _context.Events
                    .Where(x => x.Status == EventStatus.DELETED)
                    .Include(x => x.Participants)
                    .Include(x => x.Tickets)
                    .Include(e => e.Location)
                    .Include(x => x.Organizer)
                    .Include(x => x.SpeakersAndArtists)
                    .Include(x => x.PromoCodes)
                    .Include(x => x.Reviews)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Comments)
                        .ThenInclude(x => x.User)
                    .ToListAsync() ?? throw new NotFoundException("Events not found!");

                var eventDtos = _mapper.Map<IEnumerable<EventDto>>(events);

                return eventDtos;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        public async Task<EventDto> GetEventByIdAsync(int id)
        {
            try
            {
                var @event = await _context.Events
                    .Include(x => x.Participants)
                    .Include(x => x.Participants)
                    .Include(x => x.Tickets)
                    .Include(e => e.Location)
                    .Include(x => x.Organizer)
                    .Include(x => x.SpeakersAndArtists)
                    .Include(x => x.PromoCodes)
                    .Include(x => x.Reviews)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Comments)
                        .ThenInclude(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == id)
                    ?? throw new NotFoundException("Event not found!");

                var eventDto = _mapper.Map<EventDto>(@event);

                return eventDto;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        public async Task<IEnumerable<EventDto>> GetEventBySearchtermAsync(string searchTerm)
        {
            try
            {
                var @event = await _context.Events
                    .Where(x => x.Title == searchTerm || x.Description.Contains(searchTerm) &&
                                         x.Status != EventStatus.DELETED && x.Status != EventStatus.DRAFT)
                    .Include(x => x.Participants)
                    .Include(x => x.Tickets)
                    .Include(e => e.Location)
                    .Include(x => x.Organizer)
                    .Include(x => x.SpeakersAndArtists)
                    .Include(x => x.PromoCodes)
                    .Include(x => x.Reviews)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Comments)
                        .ThenInclude(x => x.User)
                    .ToListAsync()
                    ?? throw new NotFoundException($"Event can't be found with search term - {searchTerm}");

                var eventDtos = _mapper.Map<IEnumerable<EventDto>>(@event);

                return eventDtos;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        public async Task<IEnumerable<EventDto>> GetEventsByOrganizerIdAsync(int organizerId)
        {
            try
            {
                if (!_context.Organizers.Any(x => x.Id == organizerId))
                    throw new NotFoundException("Organizer not found!");

                var events = await _context.Events
                    .Where(e => e.OrganizerId == organizerId)
                    .Include(x => x.Participants)
                    .Include(x => x.Tickets)
                    .Include(e => e.Location)
                    .Include(x => x.Organizer)
                    .Include(x => x.SpeakersAndArtists)
                    .Include(x => x.PromoCodes)
                    .Include(x => x.Reviews)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Comments)
                        .ThenInclude(x => x.User)
                    .ToListAsync() ?? throw new NotFoundException("Organizer does not have any events planed.");

                var eventDtos = _mapper.Map<IEnumerable<EventDto>>(events);

                return eventDtos;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        public async Task<IEnumerable<EventDto>> GetEventsByLocationIdAsync(int locationId)
        {
            try
            {
                if (!_context.Locations.Any(x => x.Id == locationId))
                    throw new NotFoundException("Location not found!");

                var events = await _context.Events
                    .Where(e => e.LocationId == locationId)
                    .Include(x => x.Participants)
                    .Include(x => x.Tickets)
                    .Include(e => e.Location)
                    .Include(x => x.Organizer)
                    .Include(x => x.SpeakersAndArtists)
                    .Include(x => x.PromoCodes)
                    .Include(x => x.Reviews)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Comments)
                        .ThenInclude(x => x.User)
                    .ToListAsync() ?? throw new NotFoundException("Locations does not have any events related.");

                var eventDtos = _mapper.Map<IEnumerable<EventDto>>(events);

                return eventDtos;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
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
                    @event.Images = [.. (await Task.WhenAll(eventCreateDto.Images
                        .Select(async image => await _imageRepository.GenerateImageSource(image))))];
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
                @event.BookedStaff = (int)Math.Floor((@event.Location.AvailableStaff + @event.Location.BookedStaff) / ratio);
                @event.Location.AvailableStaff -= @event.BookedStaff;
                @event.Location.BookedStaff += @event.BookedStaff;
                @event.Status = EventStatus.DRAFT;

                await _context.Events.AddAsync(@event);
                await _context.SaveChangesAsync();

                var savedEvent = await _context.Events.FindAsync(@event.Id);
                var dto = _mapper.Map<EventDto>(savedEvent);

                await transaction.CommitAsync();

                return dto;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        public async Task<UserDto> AddSpeakerOrArtistOnEventAsync(int eventId, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var @event = await _context.Events.FindAsync(eventId) ?? throw new NotFoundException("Event not found!");
                var user = await _context.Users.FindAsync(userId) ?? throw new NotFoundException("User not found!");

                if (user.UserType != UserType.SPEAKER || user.UserType != UserType.ARTIST)
                    throw new BadRequestException("User is neither speaker, nor artist. User should be artist or speaker to be added on event as one!");

                if (@event.SpeakersAndArtists.Any(x => x.Id == userId))
                    throw new BadRequestException("User is already added on event as speaker or an artist!");

                @event.SpeakersAndArtists.Add(user);
                await _context.SaveChangesAsync();

                var dto = _mapper.Map<UserDto>(user);

                await transaction.CommitAsync();

                return dto;
            }
            catch (Exception ex) 
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        public async Task<string> RemoveSpeakerOrArtistFromEventAsync(int eventId, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var @event = await _context.Events.FindAsync(eventId) ?? throw new NotFoundException("Event not found!");
                var user = await _context.Users.FindAsync(userId) ?? throw new NotFoundException("User not found!");

                if (!@event.SpeakersAndArtists.Any(x => x.Id == userId))
                    throw new BadRequestException("User not found on event as speaker or an artist!");

                @event.SpeakersAndArtists.Remove(user);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return "User got successfully removed from the event!";
            }
            catch (Exception ex)
            {
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

                try
                {
                    existingEvent.Images = [.. (await Task.WhenAll(eventUpdateDto.Images
                        .Select(async image => await _imageRepository.GenerateImageSource(image))))];
                }
                catch (Exception ex)
                {
                    throw new BadRequestException(ex.Message, ex.InnerException);
                }

                _mapper.Map(eventUpdateDto, existingEvent);

                if (eventUpdateDto.Capacity != null)
                {
                    int capacityDifference = eventUpdateDto.Capacity.Value - existingEvent.Capacity;

                    if (eventUpdateDto.Capacity > existingEvent.Location.MaxCapacity)
                        throw new BadRequestException("Event capacity exceeds location’s maximum capacity!");

                    existingEvent.Location.RemainingCapacity -= capacityDifference;

                    var ratio = eventUpdateDto.Capacity == 0 ? 1 : (double)existingEvent.Location.MaxCapacity / eventUpdateDto.Capacity.Value;
                    int newBookedStaff = (int)Math.Floor((existingEvent.Location.AvailableStaff + existingEvent.Location.BookedStaff) / ratio);

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
                        .ThenInclude(x => x.Users)
                    .Include(x => x.Tickets)
                        .ThenInclude(x => x.Participants)
                    .Include(x => x.Tickets)
                        .ThenInclude(x => x.Purchases)
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

                if (newDate < DateTime.UtcNow)
                    throw new BadRequestException("Event should be planned in the future!");

                // Update the event date
                var duration = @event.EndDate - @event.StartDate;
                @event.StartDate = newDate;
                @event.EndDate = newDate + duration;
                @event.Status = EventStatus.DRAFT;

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
                @event.Location.AvailableStaff -= @event.BookedStaff;
                @event.Location.BookedStaff += @event.BookedStaff;

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
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        public async Task<bool> PublishTheEvent(int eventId)
        {
            try
            {
                var @event = await _context.Events
                    .Include(x => x.Location)
                    .FirstOrDefaultAsync(x => x.Id == eventId);
                if (@event == null) return false;

                if (@event.StartDate < DateTime.UtcNow)
                    throw new BadRequestException("Event can't be published because it's start date is overdue!");

                if (@event.Status == EventStatus.PUBLISHED)
                    throw new BadRequestException("Event is already published!");

                if (@event.Status == EventStatus.COMPLETED)
                    throw new BadRequestException("Event is already completed!");

                if (@event.Status == EventStatus.DELETED)
                {
                    @event.Location.RemainingCapacity -= @event.Capacity;
                    @event.Location.AvailableStaff -= @event.BookedStaff;
                    @event.Location.BookedStaff += @event.BookedStaff;
                }

                @event.Status = EventStatus.PUBLISHED;

                _context.Events.Update(@event);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
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
                        .ThenInclude(x => x.Users)
                    .Include(x => x.Tickets)
                        .ThenInclude(x => x.Participants)
                    .Include(x => x.Tickets)
                        .ThenInclude(x => x.Purchases)
                    .Include(x => x.PromoCodes)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (@event == null) return false;

                if (@event.Status == EventStatus.COMPLETED)
                    throw new BadRequestException("Event is already completed. It can't be deleted!");

                if (@event.Status == EventStatus.DELETED)
                    throw new BadRequestException("Event is already deleted!");

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

                @event.Status = EventStatus.DELETED;

                _context.Events.Update(@event);

                // Handle purchases: Refund if necessary
                foreach (var ticket in @event.Tickets)
                {
                    ticket.Status = TicketStatus.CANCELED;

                    if (ticket.Purchases != null)
                    {
                        foreach (var purchase in ticket.Purchases)
                        {
                            purchase.Status = PurchaseStatus.REFUNDED;
                            purchase.User.Balance += purchase.TotalAmount;
                        }
                    }
                }
                _context.Tickets.UpdateRange(@event.Tickets);
                _context.Purchases.UpdateRange(@event.Tickets.Where(t => t.Purchases != null).SelectMany(t => t.Purchases!));

                //// Because of the soft deletion
                //@event.Organizer.Events.Remove(@event);

                // Restore location capacity and staff
                @event.Location.RemainingCapacity += @event.Capacity;
                @event.Location.AvailableStaff += @event.BookedStaff;
                @event.Location.BookedStaff -= @event.BookedStaff;

                _context.Locations.Update(@event.Location);

                //// Because of the soft deletion
                //@event.Location.Events.Remove(@event);

                // Remove participants
                _context.Participants.RemoveRange(@event.Participants);

                foreach (var promoCode in @event.PromoCodes)
                {
                    _context.PromoCodes.Remove(promoCode);
                }

                // Notify participants about event cancellation
                if (@event.Status == EventStatus.DELETED)
                {
                    foreach (var participant in @event.Participants)
                    {
                        await _codeRepository.SendEventCancellationNotification(participant.User.Email, @event.Title);
                    }
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
