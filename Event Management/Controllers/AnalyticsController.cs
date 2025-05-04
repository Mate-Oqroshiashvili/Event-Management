using Event_Management.Exceptions;
using Event_Management.Models.Dtos.EventDtos;
using Event_Management.Models.Dtos.UserDtos;
using Event_Management.Repositories.EventRepositoryFolder;
using Event_Management.Repositories.UserRepositoryFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Event_Management.Controllers
{
    [Authorize] // This attribute ensures that all actions in this controller require authorization.
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IEventRepository _eventRepository;

        public AnalyticsController(IUserRepository userRepository, IEventRepository eventRepository)
        {
            _userRepository = userRepository;
            _eventRepository = eventRepository;
        }

        [HttpGet("get-user-analytics/{userId}")]
        public async Task<ActionResult<UserAnalyticsDto>> GetAnalytics(int userId)
        {
            try
            {
                var analytics = await _userRepository.GetUserAnalyticsAsync(userId);
                if (analytics == null)
                    return NotFound("User not found");

                return Ok(analytics);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ADMINISTRATOR")]
        [HttpGet("get-admin-analytics")]
        public async Task<ActionResult<AdminAnalyticsDto>> GetAdminAnalytics()
        {
            try
            {
                var analytics = await _userRepository.GetAdminAnalyticsAsync();
                return Ok(analytics);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpPost("get-analytics")]
        public async Task<ActionResult<EventAnalyticsDto>> GetEventAnalytics([FromBody] EventAnalyticsRequestDto request)
        {
            try
            {
                var analytics = await _eventRepository.GetEventAnalyticsAsync(request.OrganizerId, request.EventId);

                if (analytics == null)
                    return NotFound(new { message = "Event not found or does not belong to the organizer." });

                return Ok(analytics);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }
    }
}
