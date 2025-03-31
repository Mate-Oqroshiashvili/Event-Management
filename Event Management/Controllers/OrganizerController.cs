using Event_Management.Exceptions;
using Event_Management.Models.Dtos.OrganizerDtos;
using Event_Management.Repositories.CodeRepositoryFolder;
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
        private readonly ICodeRepository _codeRepository;

        public OrganizerController(IOrganizerRepository organizerRepository, ICodeRepository codeRepository)
        {
            _organizerRepository = organizerRepository;
            _codeRepository = codeRepository;
        }

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

        [Authorize(Roles = "ORGANIZER")]
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

                return organizerDto == null ? throw new NotFoundException("Organizer registration failed!") : Ok(new { organizerDto });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpPost("send-verification-codes-for-organizer/{organizerId}")]
        public async Task<IActionResult> SendCodes(int organizerId)
        {
            try
            {
                var result = await _codeRepository.SendCodes(organizerId);

                return result == null ? throw new BadRequestException("verification codes sending process failed!") : Ok(new { result });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpPatch("verify-organizer/{organizerId}")]
        public async Task<ActionResult<string>> VerifyOrganizer(int organizerId, string emailCode, string smsCode)
        {
            try
            {
                var organizer = await _organizerRepository.GetOrganizerByIdAsync(organizerId);

                if (organizer.User.EmailVerificationCode != emailCode || organizer.User.SmsVerificationCode != smsCode)
                    throw new BadRequestException("Verification code is not correct!");

                var verified = await _organizerRepository.VerifyOrganizerAsync(organizerId);

                return !verified ? throw new BadRequestException("Organizer verification process failed!") : Ok(new { message = "Organizer verified successfully!" });
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
