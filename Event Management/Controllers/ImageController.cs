using Event_Management.Exceptions;
using Event_Management.Repositories.ImageRepositoryFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Event_Management.Controllers
{
    [Authorize] // This attribute ensures that all actions in this controller require authorization.
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageRepository _imageRepository; // This is the repository that will handle the data access for images.

        public ImageController(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        [HttpPost("update-user-profile-picture/{userId}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<string>> UpdateProfilePicture(int userId, IFormFile formFile)
        {
            try
            {
                var result = await _imageRepository.ChangeUserProfileImage(userId, formFile);

                return result == null ? throw new BadRequestException("Profile picture changing process failed!") : Ok(result);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpPost("update-organizer-logo-image/{organizerId}")]
        public async Task<ActionResult<string>> UpdateOrganizerLogo(int organizerId, IFormFile formFile)
        {
            try
            {
                var result = await _imageRepository.ChangeOrganizerLogoImage(organizerId, formFile);

                return result == null ? throw new BadRequestException("Logo image changing process failed!") : Ok(result);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpPost("update-event-images/{eventId}")]
        public async Task<ActionResult<string>> UpdateEventImages(int eventId, [FromForm] List<string> existingImages, [FromForm] IEnumerable<IFormFile> formFile
        )
        {
            try
            {
                var result = await _imageRepository.ChangeEventImages(eventId, existingImages, formFile);

                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
