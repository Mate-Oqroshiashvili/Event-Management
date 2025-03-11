using Event_Management.Models;
using Event_Management.Models.Dtos.PurchaseDtos;

namespace Event_Management.Repositories.PurchaseRepositoryFolder
{
    public interface IPurchaseRepository
    {
        Task<IEnumerable<PurchaseDto>> GetPurchasesAsync();
        Task<PurchaseDto> GetPurchaseByIdAsync(int id);
        Task<PurchaseDto> AddPurchaseAsync(PurchaseCreateDto purchaseCreateDto);
        Task<bool> UpdatePurchaseAsync(int id, PurchaseUpdateDto purchaseUpdateDto);
        Task<bool> DeletePurchaseAsync(int id);
    }
}
