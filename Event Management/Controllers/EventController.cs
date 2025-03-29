using Event_Management.Exceptions;
using Event_Management.Models.Dtos.EventDtos;
using Event_Management.Repositories.EventRepositoryFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Event_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;

        public EventController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
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

        [HttpGet("get-event-by-search-term/{searchTerm}")]
        public async Task<ActionResult<EventDto>> GetEventBySearchTerm(string searchTerm)
        {
            try
            {
                var @event = await _eventRepository.GetEventBySearchtermAsync(searchTerm);

                return @event == null ? throw new NotFoundException("Event not found!") : Ok(new { @event });
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

        [HttpPut("reschedule-event/{eventId}")]
        public async Task<ActionResult<string>> RescheduleEvent(int eventId, DateTime dateTime)
        {
            try
            {
                var @event = await _eventRepository.RescheduleEventAsync(eventId, dateTime);

                return !@event ? throw new NotFoundException("Event not found!") : Ok(new { message = "Event rescheduled successfully!" });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

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
