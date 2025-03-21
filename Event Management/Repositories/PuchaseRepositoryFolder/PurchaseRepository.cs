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

        public async Task<IEnumerable<PurchaseDto>> GetPurchasesByUserIdAsync(int userId)
        {
            var purchases = await _context.Purchases
                .Where(p => p.UserId == userId)
                .Include(x => x.Tickets)
                .Include(x => x.User)
                .ToListAsync();

            var purchaseDtos = _mapper.Map<IEnumerable<PurchaseDto>>(purchases);

            return purchaseDtos;
        }

        public async Task<PurchaseDto> AddPurchaseAsync(PurchaseCreateDto purchaseCreateDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var ticketIds = purchaseCreateDto.Tickets.Select(tp => tp.TicketId).ToList();

                var tickets = await _context.Tickets
                    .Include(x => x.Participant)
                    .Include(x => x.Purchase)
                    .Include(x => x.Event)
                    .Include(x => x.User)
                    .Where(t => ticketIds.Contains(t.Id))
                    .ToListAsync();

                if (tickets.Any(t => t.Status != TicketStatus.AVAILABLE))
                    throw new InvalidOperationException("One or more tickets are not available for purchase.");

                if (tickets.Any(t => t.IsUsed))
                    throw new InvalidOperationException("One or more tickets have already been used.");

                var purchase = new Purchase
                {
                    UserId = purchaseCreateDto.UserId,
                    PurchaseDate = DateTime.UtcNow,
                    Status = PurchaseStatus.COMPLETED,
                    PromoCodeId = null,
                    isPromoCodeUsed = false,
                    TotalAmount = 0
                };

                foreach (var ticketRequest in purchaseCreateDto.Tickets)
                {
                    var ticket = tickets.FirstOrDefault(t => t.Id == ticketRequest.TicketId)
                        ?? throw new InvalidOperationException($"Ticket with ID {ticketRequest.TicketId} not found.");

                    if (ticketRequest.Quantity > ticket.Quantity)
                        throw new InvalidOperationException($"Not enough stock for ticket ID {ticket.Id}.");

                    purchase.TotalAmount += ticket.Price * ticketRequest.Quantity;

                    if (ticket.User == null)
                        throw new Exception("User associated with ticket can't be found!");

                    // Promo code logic
                    if (!string.IsNullOrEmpty(purchaseCreateDto.PromoCodeText))
                    {
                        var promoCode = await _context.PromoCodes
                            .FirstOrDefaultAsync(pc => pc.PromoCodeText == purchaseCreateDto.PromoCodeText &&
                                                 pc.PromoCodeStatus == PromoCodeStatus.Available &&
                                                 pc.ExpiryDate > DateTime.UtcNow)
                            ?? throw new InvalidOperationException("Invalid, expired, or unavailable promo code.");

                        UsedPromoCode usedPromoCode = new UsedPromoCode()
                        {
                            PromoCodeId = promoCode.Id,
                            PromoCode = promoCode,
                            UserId = ticket.User.Id,
                            User = ticket.User,
                            UsedDate = DateTime.UtcNow
                        };

                        if (!ticket.User.UsedPromoCodes.Any(upc => upc.PromoCodeId == usedPromoCode.PromoCodeId))
                        {
                            if (promoCode.PromoCodeAmount > 0)
                            {
                                purchase.PromoCodeId = promoCode.Id;
                                purchase.isPromoCodeUsed = true;
                                
                                ticket.User.UsedPromoCodes.Add(usedPromoCode);
                                await _context.UsedPromoCodes.AddAsync(usedPromoCode);

                                decimal discountAmount = (purchase.TotalAmount * promoCode.SaleAmountInPercentages) / 100;
                                purchase.TotalAmount -= discountAmount;

                                // Ensure the final amount is never negative
                                if (purchase.TotalAmount < 0) purchase.TotalAmount = 0;

                                promoCode.PromoCodeAmount -= 1;
                                if (promoCode.PromoCodeAmount == 0)
                                    promoCode.PromoCodeStatus = PromoCodeStatus.OutOfStock;
                            }
                            else
                            {
                                throw new Exception("Promo code is out of stock!");
                            }
                        }
                    }

                    if (ticket.User.Balance < purchase.TotalAmount)
                        throw new Exception("You don't have enought money on balance for this purchase!");

                    ticket.User.Balance -= purchase.TotalAmount;
                }

                await _context.Purchases.AddAsync(purchase);
                await _context.SaveChangesAsync(); // Save to get purchase ID

                // Handle tickets and participants
                foreach (var ticketRequest in purchaseCreateDto.Tickets)
                {
                    var ticket = tickets.First(t => t.Id == ticketRequest.TicketId);
                    var unitsToBuy = ticketRequest.Quantity;

                    ticket.Quantity -= unitsToBuy;
                    if (ticket.Quantity == 0)
                        ticket.Status = TicketStatus.SOLD_OUT;

                    ticket.PurchaseId = purchase.Id;

                    for (int i = 0; i < unitsToBuy; i++)
                    {
                        var participant = new Participant
                        {
                            UserId = purchase.UserId,
                            EventId = ticket.EventId,
                            TicketId = ticket.Id,
                            RegistrationDate = DateTime.UtcNow,
                            Attendance = false
                        };
                        await _context.Participants.AddAsync(participant);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var purchaseDto = _mapper.Map<PurchaseDto>(purchase);

                // Send emails (outside transaction block, but still inside try-catch)
                foreach (var ticketRequest in purchaseCreateDto.Tickets)
                {
                    var ticket = tickets.First(t => t.Id == ticketRequest.TicketId);
                    var ticketDto = _mapper.Map<TicketDto>(ticket);

                    try
                    {
                        var emailMessage = await _codeRepository.SendTicketToEmail(purchase.User.Email, ticketDto);
                        Console.WriteLine(emailMessage);
                    }
                    catch (Exception ex)
                    {
                        // Handle email sending failure (optional)
                        Console.WriteLine("Email sending failed: " + ex.Message);
                    }
                }

                return purchaseDto;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw; // rethrow the original exception
            }
        }

        public async Task<bool> UpdatePurchaseAsync(int id, PurchaseUpdateDto purchaseUpdateDto)
        {
            var existingPurchase = await _context.Purchases.FirstOrDefaultAsync(p => p.Id == id);
            if (existingPurchase == null) return false;

            _mapper.Map(purchaseUpdateDto, existingPurchase);

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
