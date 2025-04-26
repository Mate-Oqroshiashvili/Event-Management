using AutoMapper;
using Event_Management.Data;
using Event_Management.Exceptions;
using Event_Management.Models;
using Event_Management.Models.Dtos.PromoCodeDtos;
using Event_Management.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Event_Management.Repositories.PromoCodeRepositoryFolder
{
    public class PromoCodeRepository : IPromoCodeRepository
    {
        private readonly DataContext _context; // Database context for accessing the database
        private readonly IMapper _mapper; // AutoMapper for mapping between DTOs and entities

        public PromoCodeRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all promo codes from the database.
        public async Task<IEnumerable<PromoCodeDto>> GetPromoCodesAsync()
        {
            var promoCodes = await _context.PromoCodes
                .Include(x => x.Event)
                    .ThenInclude(x => x.Organizer)
                .ToListAsync();

            var promoCodeDtos = _mapper.Map<IEnumerable<PromoCodeDto>>(promoCodes);

            return promoCodeDtos;
        }

        /// <summary>
        /// Retrieves a promo code by its ID.
        public async Task<PromoCodeDto> GetPromoCodeByIdAsync(int id)
        {
            var promoCode = await _context.PromoCodes
                .Include(x => x.Event)
                    .ThenInclude(x => x.Organizer)
                .FirstOrDefaultAsync(x => x.Id == id);

            var promoCodeDto = _mapper.Map<PromoCodeDto>(promoCode);

            return promoCodeDto;
        }

        /// <summary>
        /// Retrieves a promo code by its search term.
        public async Task<PromoCodeDto> GetPromoCodeBySearchTermAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    throw new BadRequestException("Search term should not be empty!");

                var promoCode = await _context.PromoCodes
                    .Include(x => x.Event)
                    .FirstOrDefaultAsync(x => x.PromoCodeText.ToLower() == searchTerm.ToLower()) ?? throw new NotFoundException("Promocode not found!");

                var promoCodeDto = _mapper.Map<PromoCodeDto>(promoCode);

                return promoCodeDto;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Retrieves promo codes associated with a specific event ID.
        public async Task<IEnumerable<PromoCodeDto>> GetPromoCodesByEventIdAsync(int eventId)
        {
            var promoCodes = await _context.PromoCodes
                .Where(p => p.EventId == eventId && p.Event.Status != EventStatus.DELETED && p.Event.Status != EventStatus.DRAFT)
                .Include(x => x.Event)
                    .ThenInclude(x => x.Organizer)
                .ToListAsync();

            var promoCodeDtos = _mapper.Map<IEnumerable<PromoCodeDto>>(promoCodes);

            return promoCodeDtos;
        }

        /// <summary>
        /// Retrieves a random promo code for a user.
        public async Task<PromoCodeDto> GetRandomPromoCodeAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found.");

            if (!user.PromoCodeIsClaimable)
                throw new BadRequestException("You can claim a new promo code once every 3 days. Please try again later.");

            var promoCodes = await _context.PromoCodes
                .Include(x => x.Event)
                .Where(x => x.Event.Status == EventStatus.PUBLISHED)
                .ToListAsync();

            if (promoCodes == null || !promoCodes.Any())
                throw new NotFoundException("No promo codes available.");

            var random = new Random();
            var randomPromoCode = promoCodes[random.Next(promoCodes.Count)];

            // Map and return DTO
            var promoCodeDto = _mapper.Map<PromoCodeDto>(randomPromoCode);

            // Mark promo code as claimed
            user.PromoCodeIsClaimable = false;
            user.LastPromoClaimedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return promoCodeDto;
        }

        /// <summary>
        /// Adds a new promo code to the database.
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

            var existingPromoCode = await _context.PromoCodes
                .Where(x => x.PromoCodeText == promoCodeCreateDto.PromoCodeText)
                .FirstOrDefaultAsync();

            if (existingPromoCode != null)
                throw new BadRequestException("Promo code already exists with this promo text!");

            await _context.PromoCodes.AddAsync(promoCode);
            await _context.SaveChangesAsync();

            var promoCodeDto = _mapper.Map<PromoCodeDto>(promoCode);

            return promoCodeDto;
        }

        /// <summary>
        /// Updates an existing promo code in the database.
        public async Task<bool> UpdatePromoCodeAsync(int id, PromoCodeUpdateDto promoCodeUpdateDto)
        {
            var existingPromoCode = await _context.PromoCodes.FindAsync(id);
            if (existingPromoCode == null) return false;

            if (promoCodeUpdateDto.PromoCodeAmount <= 0)
                existingPromoCode.PromoCodeStatus = PromoCodeStatus.OutOfStock;

            _mapper.Map(promoCodeUpdateDto, existingPromoCode);

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Deletes a promo code by its ID.
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
