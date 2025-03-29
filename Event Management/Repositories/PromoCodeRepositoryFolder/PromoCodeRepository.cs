using AutoMapper;
using Event_Management.Data;
using Event_Management.Exceptions;
using Event_Management.Models;
using Event_Management.Models.Dtos.ParticipantDtos;
using Event_Management.Models.Dtos.PromoCodeDtos;
using Event_Management.Models.Enums;
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

        public async Task<PromoCodeDto> GetPromoCodeBySearchTermAsync(string searchTerm)
        {
            var promoCode = await _context.PromoCodes
                .Include(x => x.Event)
                .FirstOrDefaultAsync(x => x.PromoCodeText.Equals(searchTerm, StringComparison.CurrentCultureIgnoreCase));

            var promoCodeDto = _mapper.Map<PromoCodeDto>(promoCode);

            return promoCodeDto;
        }

        public async Task<IEnumerable<PromoCodeDto>> GetPromoCodesByEventIdAsync(int eventId)
        {
            var promoCodes = await _context.PromoCodes
                .Where(p => p.EventId == eventId && p.Event.Status != EventStatus.DELETED && p.Event.Status != EventStatus.DRAFT)
                .Include(x => x.Event)
                .ToListAsync();

            var promoCodeDtos = _mapper.Map<IEnumerable<PromoCodeDto>>(promoCodes);

            return promoCodeDtos;
        }

        public async Task<PromoCodeDto> AddPromoCodeAsync(PromoCodeCreateDto promoCodeCreateDto)
        {
            var promoCode = _mapper.Map<PromoCode>(promoCodeCreateDto);

            promoCode.PromoCodeStatus = PromoCodeStatus.Available;

            promoCode.Event = await _context.Events
                    .Include(x => x.Participants)
                    .Include(x => x.Tickets)
                    .Include(x => x.Location)
                    .Include(x => x.Organizer)
                    .FirstOrDefaultAsync(x => x.Id == promoCodeCreateDto.EventId)
                    ?? throw new NotFoundException($"Event with ID {promoCodeCreateDto.EventId} not found!");

            await _context.PromoCodes.AddAsync(promoCode);
            await _context.SaveChangesAsync();

            var promoCodeDto = _mapper.Map<PromoCodeDto>(promoCode);

            return promoCodeDto;
        }

        public async Task<bool> UpdatePromoCodeAsync(int id, PromoCodeUpdateDto promoCodeUpdateDto)
        {
            var existingPromoCode = await _context.PromoCodes.FindAsync(id);
            if (existingPromoCode == null) return false;

            if(promoCodeUpdateDto.PromoCodeAmount <= 0)
                existingPromoCode.PromoCodeStatus = PromoCodeStatus.OutOfStock;

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
