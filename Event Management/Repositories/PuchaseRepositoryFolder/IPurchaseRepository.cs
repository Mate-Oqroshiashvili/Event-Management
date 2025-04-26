using Event_Management.Models;
using Event_Management.Models.Dtos.EventDtos;
using Event_Management.Models.Dtos.PurchaseDtos;

namespace Event_Management.Repositories.PurchaseRepositoryFolder
{
    public interface IPurchaseRepository
    {
        Task<IEnumerable<PurchaseDto>> GetPurchasesAsync(); // Retrieves all purchases
        Task<PurchaseDto> GetPurchaseByIdAsync(int id); // Retrieves a purchase by its ID
        Task<IEnumerable<PurchaseDto>> GetPurchasesByUserIdAsync(int userId); // Retrieves purchases by user ID
        Task<string> AddPurchaseAsync(PurchaseCreateDto purchaseCreateDto); // Adds a new purchase
        Task<bool> UpdatePurchaseAsync(int id, PurchaseUpdateDto purchaseUpdateDto); // Updates an existing purchase
        Task<bool> DeletePurchaseAsync(int id); // Deletes a purchase by its ID
    }
}
