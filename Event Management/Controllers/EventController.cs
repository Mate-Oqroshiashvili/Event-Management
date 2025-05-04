using Event_Management.Exceptions;
using Event_Management.Models.Dtos.EventDtos;
using Event_Management.Repositories.EventRepositoryFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Event_Management.Controllers
{
    [Authorize] // This attribute ensures that all actions in this controller require authorization.
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventRepository _eventRepository; // This is the repository that will handle the data access for events.

        public EventController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        [HttpGet("get-most-popular-published-events")]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetMostPopularPublishedEvents()
        {
            try
            {
                var events = await _eventRepository.GetMostPopularPublishedEventsAsync();

                return events == null ? throw new NotFoundException("Events not found!") : Ok(new { events });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }
        
        [HttpGet("get-most-recent-published-events")]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetMostRecentPublishedEvents()
        {
            try
            {
                var events = await _eventRepository.GetMostRecentPublishedEventsAsync();

                return events == null ? throw new NotFoundException("Events not found!") : Ok(new { events });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }
        
        [HttpGet("get-published-events")]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetPublishedEvents()
        {
            try
            {
                var events = await _eventRepository.GetPublishedEventsAsync();

                return events == null ? throw new NotFoundException("Events not found!") : Ok(new { events });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpGet("get-drafted-events")]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetDraftedEvents()
        {
            try
            {
                var events = await _eventRepository.GetDraftedEventsAsync();

                return events == null ? throw new NotFoundException("Events not found!") : Ok(new { events });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpGet("get-completed-events")]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetCompletedEvents()
        {
            try
            {
                var events = await _eventRepository.GetCompletedEventsAsync();

                return events == null ? throw new NotFoundException("Events not found!") : Ok(new { events });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpGet("get-deleted-events")]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetDeletedEvents()
        {
            try
            {
                var events = await _eventRepository.GetDeletedEventsAsync();

                return events == null ? throw new NotFoundException("Events not found!") : Ok(new { events });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpGet("get-event-by-id/{eventId}")]
        public async Task<ActionResult<EventDto>> GetEventById(int eventId)
        {
            try
            {
                var @event = await _eventRepository.GetEventByIdAsync(eventId);

                return @event == null ? throw new NotFoundException("Event not found!") : Ok(new { @event });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpGet("get-events-by-search-term/{searchTerm}")]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetEventBySearchTerm(string searchTerm)
        {
            try
            {
                var events = await _eventRepository.GetEventBySearchtermAsync(searchTerm);

                return events == null ? throw new NotFoundException("Events not found!") : Ok(new { events });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpGet("get-events-by-organizer-id/{organizerId}")]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetEventsByOrganizerId(int organizerId)
        {
            try
            {
                var events = await _eventRepository.GetEventsByOrganizerIdAsync(organizerId);

                return events == null ? throw new NotFoundException("Events not found!") : Ok(new { events });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpGet("get-events-by-location-id/{locationId}")]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetEventsByLocationId(int locationId)
        {
            try
            {
                var events = await _eventRepository.GetEventsByLocationIdAsync(locationId);

                return events == null ? throw new NotFoundException("Events not found!") : Ok(new { events });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpPost("add-event")]
        public async Task<ActionResult<EventDto>> AddEvent([FromForm] EventCreateDto eventCreateDto)
        {
            try
            {
                var @event = await _eventRepository.AddEventAsync(eventCreateDto);

                return @event == null ? throw new NotFoundException("Event not found!") : Ok(new { @event });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpPost("add-speaker-or-artist-on-event/{eventId}&{userId}")]
        public async Task<ActionResult<EventDto>> AddSpeakerOrArtistOnEvent(int eventId, int userId)
        {
            try
            {
                var userDto = await _eventRepository.AddSpeakerOrArtistOnEventAsync(eventId, userId);

                return userDto == null ? throw new NotFoundException("Speaker/Artist addition failed!") : Ok(new { userDto });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpDelete("remove-speaker-or-artist-from-event/{eventId}&{userId}")]
        public async Task<ActionResult<EventDto>> FromSpeakerFromArtistOnEvent(int eventId, int userId)
        {
            try
            {
                var result = await _eventRepository.RemoveSpeakerOrArtistFromEventAsync(eventId, userId);

                return result == null ? throw new NotFoundException("Speaker/Artist removing process failed!") : Ok(new { result });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpPut("update-event/{eventId}")]
        public async Task<ActionResult<string>> UpdateEvent(int eventId, [FromForm] EventUpdateDto eventUpdateDto)
        {
            try
            {
                var @event = await _eventRepository.UpdateEventAsync(eventId, eventUpdateDto);

                return !@event ? throw new NotFoundException("Event not found!") : Ok(new { message = "Event updated successfully!" });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpPut("reschedule-event/{eventId}")]
        public async Task<ActionResult<string>> RescheduleEvent(int eventId, [FromBody] RescheduleEventDto rescheduleEventDto)
        {
            try
            {
                var @event = await _eventRepository.RescheduleEventAsync(eventId, rescheduleEventDto);

                return !@event ? throw new NotFoundException("Event not found!") : Ok(new { message = "Event rescheduled successfully!" });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpPatch("publish-event/{eventId}")]
        public async Task<ActionResult<string>> PublishEvent(int eventId)
        {
            try
            {
                var @event = await _eventRepository.PublishTheEvent(eventId);

                return !@event ? throw new NotFoundException("Event can't be published!") : Ok(new { message = "Event published successfully!" });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpDelete("remove-event/{eventId}")]
        public async Task<ActionResult<string>> RemoveEvent(int eventId)
        {
            try
            {
                var @event = await _eventRepository.DeleteEventAsync(eventId);

                return !@event ? throw new NotFoundException("Event not found!") : Ok(new { message = "Event removed successfully!" });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }
    }
}
