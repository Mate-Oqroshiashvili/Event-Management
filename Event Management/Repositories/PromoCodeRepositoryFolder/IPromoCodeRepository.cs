using Event_Management.Models.Dtos.PromoCodeDtos;

namespace Event_Management.Repositories.PromoCodeRepositoryFolder
{
    public interface IPromoCodeRepository
    {
        Task<IEnumerable<PromoCodeDto>> GetPromoCodesAsync(); // Retrieves all promo codes
        Task<PromoCodeDto> GetPromoCodeByIdAsync(int id); // Retrieves a promo code by its ID
        Task<PromoCodeDto> GetPromoCodeBySearchTermAsync(string searchTerm); // Retrieves a promo code by its search term
        Task<IEnumerable<PromoCodeDto>> GetPromoCodesByEventIdAsync(int eventId); // Retrieves promo codes by event ID
        Task<PromoCodeDto> GetRandomPromoCodeAsync(int userId); // Retrieves a random promo code for a user
        Task<PromoCodeDto> AddPromoCodeAsync(PromoCodeCreateDto promoCodeCreateDto); // Adds a new promo code
        Task<bool> UpdatePromoCodeAsync(int id, PromoCodeUpdateDto promoCodeUpdateDto); // Updates an existing promo code
        Task<bool> DeletePromoCodeAsync(int id); // Deletes a promo code by its ID
    }
}
