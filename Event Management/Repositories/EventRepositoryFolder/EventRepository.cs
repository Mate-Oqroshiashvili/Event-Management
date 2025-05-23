﻿using AutoMapper;
using Event_Management.Data;
using Event_Management.Exceptions;
using Event_Management.Models;
using Event_Management.Models.Dtos.EventDtos;
using Event_Management.Models.Dtos.UserDtos;
using Event_Management.Models.Enums;
using Event_Management.Repositories.CodeRepositoryFolder;
using Event_Management.Repositories.ImageRepositoryFolder;
using Event_Management.Web_Sockets;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Event_Management.Repositories.EventRepositoryFolder
{
    public class EventRepository : IEventRepository
    {
        private readonly DataContext _context; // Database context for accessing the database
        private readonly ICodeRepository _codeRepository; // Code repository for handling promo codes
        private readonly IImageRepository _imageRepository; // Image repository for handling image uploads
        private readonly IHubContext<EventHub> _hubContext; // Hub context for sending messages to clients
        private readonly IMapper _mapper; // AutoMapper instance for mapping between DTOs and entities

        public EventRepository(DataContext context, ICodeRepository codeRepository, IImageRepository imageRepository, IHubContext<EventHub> hubContext, IMapper mapper)
        {
            _context = context;
            _codeRepository = codeRepository;
            _imageRepository = imageRepository;
            _hubContext = hubContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves the most popular published events from the database.
        public async Task<IEnumerable<EventDto>> GetMostPopularPublishedEventsAsync()
        {
            try
            {
                var events = await _context.Events
                    .Where(x => x.Status == EventStatus.PUBLISHED && x.Tickets.Any())
                    .Include(x => x.Reviews)
                        .ThenInclude(x => x.User)
                    .OrderByDescending(x => x.Tickets.Sum(t => t.Purchases.Count))
                    .Take(3)
                    .ToListAsync() ?? throw new NotFoundException("Popular events not found!"); ;

                var eventDtos = _mapper.Map<IEnumerable<EventDto>>(events);

                return eventDtos;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Retrieves the most recent published events from the database.
        public async Task<IEnumerable<EventDto>> GetMostRecentPublishedEventsAsync()
        {
            try
            {
                var events = await _context.Events
                    .Where(x => x.Status == EventStatus.PUBLISHED && x.StartDate != null)
                    .Include(x => x.Reviews)
                        .ThenInclude(x => x.User)
                    .OrderByDescending(x => x.StartDate)
                    .Take(3)
                    .ToListAsync() ?? throw new NotFoundException("Recent events not found!"); ;

                var eventDtos = _mapper.Map<IEnumerable<EventDto>>(events);

                return eventDtos;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Retrieves all published events from the database.
        public async Task<IEnumerable<EventDto>> GetPublishedEventsAsync()
        {
            try
            {
                var events = await _context.Events
                    .Where(x => x.Status == EventStatus.PUBLISHED)
                    //.Include(x => x.Organizer)
                    .Include(x => x.Reviews)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Tickets)
                        .ThenInclude(x => x.Purchases)
                    .ToListAsync() ?? throw new NotFoundException("Events not found!");

                var eventDtos = _mapper.Map<IEnumerable<EventDto>>(events);

                return eventDtos;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Retrieves all drafted events from the database.
        public async Task<IEnumerable<EventDto>> GetDraftedEventsAsync()
        {
            try
            {
                var events = await _context.Events
                    .Where(x => x.Status == EventStatus.DRAFT)
                    .Include(x => x.Organizer)
                    .Include(x => x.Tickets)
                    .Include(x => x.SpeakersAndArtists)
                    .Include(x => x.Reviews)
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

        /// <summary>
        /// Retrieves all completed events from the database.
        public async Task<IEnumerable<EventDto>> GetCompletedEventsAsync()
        {
            try
            {
                var events = await _context.Events
                    .Where(x => x.Status == EventStatus.COMPLETED)
                    .Include(x => x.Organizer)
                    .Include(x => x.Reviews)
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

        /// <summary>
        /// Retrieves all deleted events from the database.
        public async Task<IEnumerable<EventDto>> GetDeletedEventsAsync()
        {
            try
            {
                var events = await _context.Events
                    .Where(x => x.Status == EventStatus.DELETED)
                    .Include(x => x.Organizer)
                    .Include(x => x.Tickets)
                    .Include(x => x.Reviews)
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

        /// <summary>
        /// Retrieves an event by its ID from the database.
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
                        .ThenInclude(x => x.User)
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

        /// <summary>
        /// Retrieves events by a search term from the database.
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
                        .ThenInclude(x => x.User)
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

        /// <summary>
        /// Retrieves events by organizer ID from the database.
        public async Task<IEnumerable<EventDto>> GetEventsByOrganizerIdAsync(int organizerId)
        {
            try
            {
                if (!_context.Organizers.Any(x => x.Id == organizerId))
                    throw new NotFoundException("Organizer not found!");

                var events = await _context.Events
                    .Where(e => e.OrganizerId == organizerId)
                    .Include(x => x.Reviews)
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

        /// <summary>
        /// Retrieves events by location ID from the database.
        public async Task<IEnumerable<EventDto>> GetEventsByLocationIdAsync(int locationId)
        {
            try
            {
                if (!_context.Locations.Any(x => x.Id == locationId))
                    throw new NotFoundException("Location not found!");

                var events = await _context.Events
                    .Where(e => e.LocationId == locationId)
                    .Include(x => x.Reviews)
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

        /// <summary>
        /// Retrieves events by user ID from the database.
        public async Task<EventAnalyticsDto> GetEventAnalyticsAsync(int organizerId, int eventId)
        {
            var eventData = await _context.Events
                .Include(e => e.Participants)
                .Include(e => e.Tickets).ThenInclude(t => t.Purchases)
                .Include(e => e.PromoCodes)
                .Include(e => e.Reviews)
                .Include(e => e.Comments)
                .Where(e => e.Id == eventId && e.OrganizerId == organizerId && e.Status != EventStatus.DELETED)
                .Select(e => new EventAnalyticsDto
                {
                    EventId = e.Id,
                    Title = e.Title,

                    // Attendance
                    TotalParticipants = e.Participants.Count(),
                    AttendedParticipants = e.Participants.Count(p => p.Attendance == true),
                    OccupancyRate = e.Capacity > 0
                        ? (double)e.Participants.Count() / e.Capacity * 100
                        : 0.0,

                    // Tickets by Type
                    VIPTicketCount = e.Tickets.Where(t => t.Type == TicketType.VIP).Sum(t => t.Quantity),
                    BasicTicketCount = e.Tickets.Where(t => t.Type == TicketType.BASIC).Sum(t => t.Quantity),
                    EarlyBirdTicketCount = e.Tickets.Where(t => t.Type == TicketType.EARLYBIRD).Sum(t => t.Quantity),
                    TotalTicketQuantity = e.Tickets.Sum(t => t.Quantity),

                    // Financials
                    TotalTicketsSold = e.Participants.Count(),

                    TotalRevenue = e.Tickets
                        .SelectMany(t => t.Purchases)
                        .Distinct()
                        .Sum(p => p.TotalAmount),

                    // Promo Codes
                    PromoCodesUsed = e.Tickets
                        .SelectMany(t => t.Purchases)
                        .Where(p => p.isPromoCodeUsed)
                        .Select(p => p.Id)
                        .Distinct()
                        .Count(),

                    PurchasesWithPromoCode = e.Tickets
                        .SelectMany(t => t.Purchases)
                        .Where(p => p.isPromoCodeUsed)
                        .Select(p => p.Id)
                        .Distinct()
                        .Count(),

                    PurchasesWithoutPromoCode = e.Tickets
                        .SelectMany(t => t.Purchases)
                        .Where(p => !p.isPromoCodeUsed)
                        .Select(p => p.Id)
                        .Distinct()
                        .Count(),

                    AvailablePromoCodes = e.PromoCodes.Count(pc => pc.PromoCodeStatus == PromoCodeStatus.Available),
                    OutOfStockPromoCodes = e.PromoCodes.Count(pc => pc.PromoCodeStatus == PromoCodeStatus.OutOfStock),
                    TotalPromoCodes = e.PromoCodes.Count(),

                    // Reviews & Comments
                    ReviewCount = e.Reviews.Count(),
                    AverageRating = e.Reviews.Any() ? e.Reviews.Average(r => r.StarCount) : 0.0,
                    CommentCount = e.Comments.Count(),

                    FiveStarCount = e.Reviews.Count(r => r.StarCount == 5),
                    FourStarCount = e.Reviews.Count(r => r.StarCount == 4),
                    ThreeStarCount = e.Reviews.Count(r => r.StarCount == 3),
                    TwoStarCount = e.Reviews.Count(r => r.StarCount == 2),
                    OneStarCount = e.Reviews.Count(r => r.StarCount == 1),
                })
                .FirstOrDefaultAsync();

            return eventData;
        }

        /// <summary>
        /// Adds a new event to the database.
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

                // Check if the organizer is verified
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

                // Check if the event capacity is valid
                if (@event.Capacity <= 0)
                    throw new BadRequestException("Event capacity must be greater than zero!");

                // Check if the event capacity exceeds the location's maximum capacity
                if (@event.Capacity > @event.Location.RemainingCapacity)
                    throw new BadRequestException("Event capacity exceeds location’s maximum capacity!");

                // Update location's remaining capacity and booked staff
                @event.Location.RemainingCapacity -= @event.Capacity;
                var ratio = @event.Capacity == 0 ? 1 : (double)@event.Location.MaxCapacity / @event.Capacity;
                @event.BookedStaff = (int)Math.Floor((@event.Location.AvailableStaff + @event.Location.BookedStaff) / ratio);
                @event.Location.AvailableStaff -= @event.BookedStaff;
                @event.Location.BookedStaff += @event.BookedStaff;

                // Set the event status to DRAFT
                @event.Status = EventStatus.DRAFT;

                // Add the event to the database
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

        /// <summary>
        /// Adds a speaker or artist to an event.
        public async Task<UserDto> AddSpeakerOrArtistOnEventAsync(int eventId, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var @event = await _context.Events.FindAsync(eventId) ?? throw new NotFoundException("Event not found!");
                var user = await _context.Users.FindAsync(userId) ?? throw new NotFoundException("User not found!");

                if (user.UserType != UserType.SPEAKER && user.UserType != UserType.ARTIST)
                    throw new BadRequestException("User is neither speaker, nor artist. User should be artist or speaker to be added on event as one!");

                if (@event.SpeakersAndArtists.Any(x => x.Id == userId))
                    throw new BadRequestException("User is already added on event!");

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

        /// <summary>
        /// Removes a speaker or artist from an event.
        public async Task<string> RemoveSpeakerOrArtistFromEventAsync(int eventId, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var @event = await _context.Events.FindAsync(eventId) ?? throw new NotFoundException("Event not found!");
                var user = await _context.Users.FindAsync(userId) ?? throw new NotFoundException("User not found!");

                if (!@event.SpeakersAndArtists.Any(x => x.Id == userId))
                    throw new BadRequestException("User not found on event!");

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

        /// <summary>
        /// Updates an existing event in the database.
        public async Task<bool> UpdateEventAsync(int id, EventUpdateDto eventUpdateDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingEvent = await _context.Events
                    .Include(x => x.Location)
                    .Include(x => x.Organizer)
                    .FirstOrDefaultAsync(x => x.Id == id)
                    ?? throw new NotFoundException("Event does not exist!");

                if (!existingEvent.Organizer.IsVerified)
                    throw new BadRequestException("Organizer is not verified!");

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

                var @event = await _context.Events.FindAsync(id);

                await _hubContext.Clients.All.SendAsync("EventUpdated", @event?.Title);

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Reschedules an existing event in the database.
        public async Task<bool> RescheduleEventAsync(int id, RescheduleEventDto rescheduleEventDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (rescheduleEventDto == null || rescheduleEventDto?.NewDate == null)
                    throw new NotFoundException("Missing date");

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

                if (rescheduleEventDto.NewDate < DateTime.UtcNow)
                    throw new BadRequestException($"Event should be planned in the future!");

                var wasDeleted = @event.Status == EventStatus.DELETED;

                // Update the event date
                var duration = @event.EndDate - @event.StartDate;
                @event.StartDate = rescheduleEventDto.NewDate;
                @event.EndDate = rescheduleEventDto.NewDate + duration;
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

                if (wasDeleted)
                {
                    Console.WriteLine("Event is deleted, updating capacities...");
                    if (@event.Capacity > @event.Location.RemainingCapacity)
                        throw new BadRequestException($"Event capacity exceeds location’s maximum capacity! Event Capacity {@event.Capacity}, Location Capacity: {@event.Location.RemainingCapacity}.");

                    @event.Location.RemainingCapacity -= @event.Capacity;
                    @event.Location.AvailableStaff -= @event.BookedStaff;
                    @event.Location.BookedStaff += @event.BookedStaff;
                }
                else
                {
                    Console.WriteLine("Event is not deleted, no capacity update needed.");
                }

                _context.Events.Update(@event);

                // Notify participants (pseudo-code, replace with real email service)
                foreach (var participant in @event.Participants)
                {
                    await _codeRepository.SendEventRescheduleNotification(participant.User.Email, @event.Title, rescheduleEventDto.NewDate);
                }

                await _context.SaveChangesAsync();

                var eventForSocket = await _context.Events.FindAsync(id);

                await _hubContext.Clients.All.SendAsync("EventRescheduled", @event?.Title);

                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Publishes an event in the database.
        public async Task<bool> PublishTheEvent(int eventId)
        {
            try
            {
                var @event = await _context.Events
                    .Include(x => x.Participants)
                    .Include(x => x.Tickets)
                    .Include(e => e.Location)
                    .Include(x => x.Organizer)
                        .ThenInclude(x => x.User)
                    .Include(x => x.SpeakersAndArtists)
                    .FirstOrDefaultAsync(x => x.Id == eventId);
                if (@event == null) return false;

                if (!@event.Tickets.Any())
                    throw new BadRequestException("No tickets found. Add tickets to upload the event!");

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

                // Notify clients about the new event
                await _hubContext.Clients.All.SendAsync("EventCreated", @event.Title);

                return true;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Deletes an event from the database.
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

                await _hubContext.Clients.All.SendAsync("EventDeleted", @event?.Title);

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
