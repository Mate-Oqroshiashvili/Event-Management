using Event_Management.Exceptions;
using Event_Management.Models;
using Event_Management.Models.Dtos.PurchaseDtos;
using Event_Management.Repositories.PurchaseRepositoryFolder;
using Microsoft.AspNetCore.Mvc;

namespace Event_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseRepository _purchaseRepository;

        public PurchaseController(IPurchaseRepository purchaseRepository)
        {
            _purchaseRepository = purchaseRepository;
        }

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
