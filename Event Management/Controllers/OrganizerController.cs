using Event_Management.Exceptions;
using Event_Management.Models.Dtos.OrganizerDtos;
using Event_Management.Models.Dtos.UserDtos;
using Event_Management.Repositories.OrganizerRepositoryFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Event_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizerController : ControllerBase
    {
        private readonly IOrganizerRepository _organizerRepository;

        public OrganizerController(IOrganizerRepository organizerRepository)
        {
            _organizerRepository = organizerRepository;
        }

        [Authorize(Roles = "ADMINISTRATOR")]
        [HttpGet("get-all-organizers")]
        public async Task<ActionResult<IEnumerable<OrganizerDto>>> GetAllOrganizers()
        {
            try
            {
                var organizerDtos = await _organizerRepository.GetOrganizersAsync();

                return organizerDtos == null ? throw new NotFoundException("Organizers can't be found!") : Ok(new { organizerDtos });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpGet("get-organizer-by-id/{organizerId}")]
        public async Task<ActionResult<OrganizerDto>> GetOrganizerById(int organizerId)
        {
            try
            {
                var organizerDto = await _organizerRepository.GetOrganizerByIdAsync(organizerId);

                return organizerDto == null ? throw new NotFoundException("Organizer can't be found!") : Ok(new { organizerDto });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "BASIC")]
        [HttpPost("register-user-as-organizer")]
        public async Task<ActionResult<OrganizerDto>> RegisterUserAsOrganizer([FromForm] OrganizerCreateDto organizerCreateDto)
        {
            try
            {
                var organizerDto = await _organizerRepository.AddOrganizerAsync(organizerCreateDto);

                return organizerDto == null ? throw new NotFoundException("Organizer registration failed!") : Ok(new {organizerDto});
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpPost("add-organizer-on-specific-location/{organizerId}&{locationId}")]
        public async Task<ActionResult<string>> AddOrganizerOnSpecificLocation(int organizerId, int locationId)
        {
            try
            {
                var result = await _organizerRepository.AddOrganizerOnLocationAsync(organizerId, locationId);

                return result == null ? throw new NotFoundException("Organizer could not be linked to the location!") : Ok(new { result });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpPut("update-organizer/{organizerId}")]
        public async Task<ActionResult<string>> UpdateUserAsOrganizer(int organizerId, [FromForm] OrganizerUpdateDto organizerUpdateDto)
        {
            try
            {
                var isUpdated = await _organizerRepository.UpdateOrganizerAsync(organizerId, organizerUpdateDto);

                return !isUpdated ? throw new NotFoundException("Organizer update process failed!") : Ok(new { message = "Organizer updated successfully!" });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpDelete("remove-organizer/{organizerId}")]
        public async Task<ActionResult<string>> RemoveUserAsOrganizer(int organizerId)
        {
            try
            {
                var isRemoved = await _organizerRepository.DeleteOrganizerAsync(organizerId);

                return !isRemoved ? throw new NotFoundException("Organizer deletion process failed!") : Ok(new { message = "Organizer removed successfully!" });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpDelete("remove-organizer-from-specific-location/{organizerId}&{locationId}")]
        public async Task<ActionResult<string>> RemoveOrganizerFromSpecificLocation(int organizerId, int locationId)
        {
            try
            {
                var result = await _organizerRepository.RemoveOrganizerFromLocationAsync(organizerId, locationId);

                return result == null ? throw new NotFoundException("Organizer could not be removed from the location!") : Ok(new { result });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }
    }
}
