using Event_Management.Exceptions;
using Event_Management.Models.Dtos.PromoCodeDtos;
using Event_Management.Repositories.PromoCodeRepositoryFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Event_Management.Controllers
{
    [Authorize] // This attribute ensures that all actions in this controller require authorization.
    [Route("api/[controller]")]
    [ApiController]
    public class PromoCodeController : ControllerBase
    {
        private readonly IPromoCodeRepository _promoCodeRepository; // This is the repository that will handle the data access for promo codes.

        public PromoCodeController(IPromoCodeRepository promoCodeRepository)
        {
            _promoCodeRepository = promoCodeRepository;
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpGet("get-all-promo-codes")]
        public async Task<ActionResult<IEnumerable<PromoCodeDto>>> GetAllPromoCodes()
        {
            try
            {
                var promoCodes = await _promoCodeRepository.GetPromoCodesAsync();

                return promoCodes == null ? throw new NotFoundException("Promo codes not found!") : Ok(new { promoCodes });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpGet("get-promo-code-by-id/{promoCodeId}")]
        public async Task<ActionResult<PromoCodeDto>> GetPromoCodeById(int promoCodeId)
        {
            try
            {
                var promoCode = await _promoCodeRepository.GetPromoCodeByIdAsync(promoCodeId);

                return promoCode == null ? throw new NotFoundException("Promo code not found!") : Ok(new { promoCode });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpGet("get-promo-code-by-search-term/{searchTerm}")]
        public async Task<ActionResult<PromoCodeDto>> GetPromoCodeBySearchTerm(string searchTerm)
        {
            try
            {
                var promoCode = await _promoCodeRepository.GetPromoCodeBySearchTermAsync(searchTerm);

                return promoCode == null ? throw new NotFoundException("Promo code not found!") : Ok(new { promoCode });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpGet("get-promo-codes-by-event-id/{eventId}")]
        public async Task<ActionResult<IEnumerable<PromoCodeDto>>> GetPromoCodesByEventId(int eventId)
        {
            try
            {
                var promoCodes = await _promoCodeRepository.GetPromoCodesByEventIdAsync(eventId);

                return promoCodes == null ? throw new NotFoundException("Promo codes not found!") : Ok(new { promoCodes });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpGet("get-random-promo-code/{userId}")]
        public async Task<ActionResult<PromoCodeDto>> GetRandomPromoCode(int userId)
        {
            try
            {
                var promoCode = await _promoCodeRepository.GetRandomPromoCodeAsync(userId);

                return promoCode == null ? throw new NotFoundException("Promo code not found!") : Ok(new { promoCode });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpPost("create-promo-code")]
        public async Task<ActionResult<PromoCodeDto>> CreatePromoCode([FromBody] PromoCodeCreateDto promoCodeCreateDto)
        {
            try
            {
                var promoCode = await _promoCodeRepository.AddPromoCodeAsync(promoCodeCreateDto);

                return promoCode == null ? throw new NotFoundException("Promo code creation process failed!") : Ok(new { promoCode });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpPut("update-promo-code/{promoCodeId}")]
        public async Task<ActionResult<string>> UpdatePromoCode(int promoCodeId, [FromBody] PromoCodeUpdateDto promoCodeUpdateDto)
        {
            try
            {
                var isUpdated = await _promoCodeRepository.UpdatePromoCodeAsync(promoCodeId, promoCodeUpdateDto);

                return !isUpdated ? throw new NotFoundException("Promo code update process failed!") : Ok(new { message = "Promo code updated successfully!" });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpDelete("remove-promo-code/{promoCodeId}")]
        public async Task<ActionResult<string>> RemovePromoCode(int promoCodeId)
        {
            try
            {
                var isRemoved = await _promoCodeRepository.DeletePromoCodeAsync(promoCodeId);

                return !isRemoved ? throw new NotFoundException("Promo code deletion process failed!") : Ok(new { message = "Promo code removed successfully!" });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }
    }
}
