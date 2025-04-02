using Event_Management.Exceptions;
using Event_Management.Repositories.ImageRepositoryFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Event_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageRepository _imageRepository;

        public ImageController(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        [Authorize]
        [HttpPost("update-user-profile-picture/{userId}")]
        public async Task<ActionResult<string>> UpdateProfilePicture(int userId, IFormFile formFile)
        {
            try
            {
                var result = await _imageRepository.ChangeUserProfileImage(userId, formFile);

                return result != null ? throw new BadRequestException("Profile picture changing process failed!") : Ok(result);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "Organizer")]
        [HttpPost("update-organizer-logo-image/{organizerId}")]
        public async Task<ActionResult<string>> UpdateOrganizerLogo(int organizerId, IFormFile formFile)
        {
            try
            {
                var result = await _imageRepository.ChangeOrganizerLogoImage(organizerId, formFile);

                return result != null ? throw new BadRequestException("Logo image changing process failed!") : Ok(result);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "Organizer")]
        [HttpPost("update-event-images/{eventId}")]
        public async Task<ActionResult<string>> UpdateEventImages(int eventId, IEnumerable<IFormFile> formFile)
        {
            try
            {
                var result = await _imageRepository.ChangeEventImages(eventId, formFile);

                return result != null ? throw new BadRequestException("Event images changing process failed!") : Ok(result);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }
    }
}
