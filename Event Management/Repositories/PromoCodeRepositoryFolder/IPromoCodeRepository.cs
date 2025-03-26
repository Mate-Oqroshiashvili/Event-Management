using Event_Management.Models.Dtos.PromoCodeDtos;

namespace Event_Management.Repositories.PromoCodeRepositoryFolder
{
    public interface IPromoCodeRepository
    {
        Task<IEnumerable<PromoCodeDto>> GetPromoCodesAsync();
        Task<PromoCodeDto> GetPromoCodeByIdAsync(int id);
        Task<PromoCodeDto> GetPromoCodeBySearchTermAsync(string searchTerm);
        Task<IEnumerable<PromoCodeDto>> GetPromoCodesByEventIdAsync(int eventId);
        Task<PromoCodeDto> AddPromoCodeAsync(PromoCodeCreateDto promoCodeCreateDto);
        Task<bool> UpdatePromoCodeAsync(int id, PromoCodeUpdateDto promoCodeUpdateDto);
        Task<bool> DeletePromoCodeAsync(int id);
    }
}
