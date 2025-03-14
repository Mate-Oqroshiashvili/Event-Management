using AutoMapper;
using Event_Management.Data;
using Event_Management.Models;
using Event_Management.Models.Dtos.PurchaseDtos;
using Event_Management.Models.Dtos.TicketDtos;
using Event_Management.Models.Enums;
using Event_Management.Repositories.CodeRepositoryFolder;
using Microsoft.EntityFrameworkCore;

namespace Event_Management.Repositories.PurchaseRepositoryFolder
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly DataContext _context;
        private readonly ICodeRepository _codeRepository;
        private readonly IMapper _mapper;

        public PurchaseRepository(DataContext context, ICodeRepository codeRepository, IMapper mapper)
        {
            _context = context;
            _codeRepository = codeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PurchaseDto>> GetPurchasesAsync()
        {
            var purchases = await _context.Purchases
                .Include(x => x.Tickets)
                .Include(x => x.User)
                .ToListAsync();

            var purchaseDtos = _mapper.Map<IEnumerable<PurchaseDto>>(purchases);

            return purchaseDtos;
        }

        public async Task<PurchaseDto> GetPurchaseByIdAsync(int id)
        {
            var purchase = await _context.Purchases
                .Include(x => x.Tickets)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            var purchaseDto = _mapper.Map<PurchaseDto>(purchase);

            return purchaseDto;
        }

        public async Task<PurchaseDto> AddPurchaseAsync(PurchaseCreateDto purchaseCreateDto)
        {
            var tickets = await _context.Tickets
                .Where(t => purchaseCreateDto.TicketIds.Contains(t.Id))
                .ToListAsync();

            if (tickets.Any(t => t.Status != TicketStatus.AVAILABLE))
            {
                throw new InvalidOperationException("One or more tickets are not available for purchase.");
            }

            var purchase = _mapper.Map<Purchase>(purchaseCreateDto);
            foreach (var ticket in tickets)
            {
                // Check if the ticket is already assigned to another purchase
                if (ticket.PurchaseId != null && ticket.PurchaseId != purchase.Id)
                {
                    throw new InvalidOperationException("One or more tickets have already been purchased.");
                }
                ticket.PurchaseId = purchase.Id;  // Assign the purchase ID
            }

            purchase.Tickets = tickets;
            purchase.TotalAmount = tickets.Sum(t => t.Price * t.Quantity);

            if (!string.IsNullOrEmpty(purchaseCreateDto.PromoCodeText))
            {
                var promoCode = await _context.PromoCodes
                    .FirstOrDefaultAsync(pc => pc.PromoCodeText == purchaseCreateDto.PromoCodeText && pc.PromoCodeStatus == PromoCodeStatus.Available) ?? throw new InvalidOperationException("Invalid or expired promo code.");

                purchase.PromoCodeId = promoCode.Id;
                purchase.isPromoCodeUsed = true;
                // Calculate discount if needed and adjust total amount
                purchase.TotalAmount -= (purchase.TotalAmount * promoCode.SaleAmountInPercentages) / 100;
            }

            await _context.Purchases.AddAsync(purchase);
            await _context.SaveChangesAsync();

            var purchaseDto = _mapper.Map<PurchaseDto>(purchase);

            foreach (var ticket in tickets)
            {
                var ticketDto = _mapper.Map<TicketDto>(ticket);
                var emailMessage = await _codeRepository.SendTicketToEmail(purchase.User.Email, ticketDto);
                Console.WriteLine(emailMessage);
            }

            return purchaseDto;
        }

        public async Task<bool> UpdatePurchaseAsync(int id, PurchaseUpdateDto purchaseUpdateDto)
        {
            var existingPurchase = await _context.Purchases.FirstOrDefaultAsync(p => p.Id == id);
            if (existingPurchase == null) return false;

            var purchase = _mapper.Map(purchaseUpdateDto, existingPurchase);

            _context.Purchases.Update(existingPurchase);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeletePurchaseAsync(int id)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null) return false;

            _context.Purchases.Remove(purchase);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
