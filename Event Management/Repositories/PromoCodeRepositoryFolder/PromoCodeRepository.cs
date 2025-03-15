using AutoMapper;
using Event_Management.Data;
using Event_Management.Models;
using Event_Management.Models.Dtos.PromoCodeDtos;
using Microsoft.EntityFrameworkCore;

namespace Event_Management.Repositories.PromoCodeRepositoryFolder
{
    public class PromoCodeRepository : IPromoCodeRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public PromoCodeRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PromoCodeDto>> GetPromoCodesAsync()
        {
            var promoCodes = await _context.PromoCodes
                .Include(x => x.Event)
                .ToListAsync();

            var promoCodeDtos = _mapper.Map<IEnumerable<PromoCodeDto>>(promoCodes);

            return promoCodeDtos;
        }

        public async Task<PromoCodeDto> GetPromoCodeByIdAsync(int id)
        {
            var promoCode = await _context.PromoCodes
                .Include(x => x.Event)
                .FirstOrDefaultAsync(x => x.Id == id);

            var promoCodeDto = _mapper.Map<PromoCodeDto>(promoCode);

            return promoCodeDto;
        }

        public async Task<PromoCodeDto> AddPromoCodeAsync(PromoCodeCreateDto promoCodeCreateDto)
        {
            var promoCode = _mapper.Map<PromoCode>(promoCodeCreateDto);

            await _context.PromoCodes.AddAsync(promoCode);
            await _context.SaveChangesAsync();

            var promoCodeDto = _mapper.Map<PromoCodeDto>(promoCode);

            return promoCodeDto;
        }

        public async Task<bool> UpdatePromoCodeAsync(int id, PromoCodeUpdateDto promoCodeUpdateDto)
        {
            var existingPromoCode = await _context.PromoCodes.FindAsync(id);
            if (existingPromoCode == null) return false;

            _mapper.Map(promoCodeUpdateDto, existingPromoCode);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePromoCodeAsync(int id)
        {
            var promoCode = await _context.PromoCodes.FindAsync(id);
            if (promoCode == null) return false;

            _context.PromoCodes.Remove(promoCode);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
