using Event_Management.Exceptions;
using Event_Management.Models.Dtos.PurchaseDtos;
using Event_Management.Repositories.PurchaseRepositoryFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Event_Management.Controllers
{
    [Authorize] // This attribute ensures that all actions in this controller require authorization.
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseRepository _purchaseRepository; // This is the repository that will handle the data access for purchases.

        public PurchaseController(IPurchaseRepository purchaseRepository)
        {
            _purchaseRepository = purchaseRepository;
        }

        [Authorize(Roles = "ORGANIZER")]
        [HttpGet("get-all-purchases")]
        public async Task<ActionResult<IEnumerable<PurchaseDto>>> GetAllPurchases()
        {
            try
            {
                var purchases = await _purchaseRepository.GetPurchasesAsync();

                return purchases == null ? throw new NotFoundException("Purchases not found!") : Ok(new { purchases });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpGet("get-one-purchase-by-id/{purchaseId}")]
        public async Task<ActionResult<PurchaseDto>> GetOnePurchaseById(int purchaseId)
        {
            try
            {
                var purchase = await _purchaseRepository.GetPurchaseByIdAsync(purchaseId);

                return purchase == null ? throw new NotFoundException("Purchase not found!") : Ok(new { purchase });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [HttpGet("get-purchases-by-user-id/{userId}")]
        public async Task<ActionResult<IEnumerable<PurchaseDto>>> GetPurchasesByUserId(int userId)
        {
            try
            {
                var purchases = await _purchaseRepository.GetPurchasesByUserIdAsync(userId);

                return purchases == null ? throw new NotFoundException("Purchases not found!") : Ok(new { purchases });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        [Authorize(Roles = "BASIC,PARTICIPANT")]
        [HttpPost("purchase-ticket")]
        public async Task<ActionResult<PurchaseDto>> PurchaseTicket([FromBody] PurchaseCreateDto purchaseCreateDto)
        {
            try
            {
                var purchaseDto = await _purchaseRepository.AddPurchaseAsync(purchaseCreateDto);

                return purchaseDto == null ? throw new NotFoundException("Purchase process failed!") : Ok(new { purchaseDto });
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }
    }
}
