using AutoMapper;
using Event_Management.Data;
using Event_Management.Models;
using Event_Management.Models.Dtos.PurchaseDtos;
using Microsoft.EntityFrameworkCore;

namespace Event_Management.Repositories.PurchaseRepositoryFolder
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public PurchaseRepository(DataContext context, IMapper mapper)
        {
            _context = context;
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

            var purchase = _mapper.Map<Purchase>(purchaseCreateDto);
            purchase.Tickets = tickets;

            await _context.Purchases.AddAsync(purchase);
            await _context.SaveChangesAsync();

            var purchaseDto = _mapper.Map<PurchaseDto>(purchase);

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
