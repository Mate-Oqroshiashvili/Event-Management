using AutoMapper;
using Event_Management.Data;
using Event_Management.Exceptions;
using Event_Management.Models;
using Event_Management.Models.Dtos.PurchaseDtos;
using Event_Management.Models.Dtos.TicketDtos;
using Event_Management.Models.Enums;
using Event_Management.Repositories.CodeRepositoryFolder;
using Microsoft.EntityFrameworkCore;
using QRCoder;

namespace Event_Management.Repositories.PurchaseRepositoryFolder
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly DataContext _context;
        private readonly ICodeRepository _codeRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _environment;

        public PurchaseRepository(DataContext context, ICodeRepository codeRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _codeRepository = codeRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _environment = webHostEnvironment;
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

        public async Task<string> AddPurchaseAsync(PurchaseCreateDto purchaseCreateDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var ticketIds = purchaseCreateDto.Tickets.Select(tp => tp.TicketId).ToList();

                var tickets = await _context.Tickets
                    .Include(x => x.Participants)
                    .Include(x => x.Purchases)
                    .Include(x => x.Event)
                    .Include(x => x.Users)
                    .Where(t => ticketIds.Contains(t.Id))
                    .ToListAsync();

                if (tickets.Any(t => t.Status != TicketStatus.AVAILABLE))
                    throw new InvalidOperationException("One or more tickets are not available for purchase.");

                if (tickets.Any(t => t.Participants.Any(x => x.IsUsed)))
                    throw new InvalidOperationException("One or more tickets have already been used.");

                var purchase = new Purchase
                {
                    UserId = purchaseCreateDto.UserId,
                    PurchaseDate = DateTime.UtcNow,
                    Status = PurchaseStatus.PENDING,
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

                    if (ticket.Users == null)
                        throw new Exception("User associated with ticket can't be found!");

                    ticket.Purchases.Add(purchase);

                    var user = await _context.Users
                        .Include(t => t.UsedPromoCodes)
                        .FirstOrDefaultAsync(x => x.Id == purchaseCreateDto.UserId)
                        ?? throw new NotFoundException("User can't be found in purchase!");

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
                            UserId = user.Id,
                            User = user,
                            UsedDate = DateTime.UtcNow
                        };

                        if (!user.UsedPromoCodes.Any(upc => upc.PromoCodeId == usedPromoCode.PromoCodeId))
                        {
                            if (promoCode.PromoCodeAmount > 0)
                            {
                                purchase.PromoCodeId = promoCode.Id;
                                purchase.isPromoCodeUsed = true;

                                user.UsedPromoCodes.Add(usedPromoCode);
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
                                throw new BadRequestException("Promo code is out of stock!");
                            }
                        }
                        else
                        {
                            throw new BadRequestException("Promo code is already used on this user account!");
                        }
                    }

                    if (user.Balance < purchase.TotalAmount)
                        throw new Exception("You don't have enought money on balance for this purchase!");

                    user.Balance -= purchase.TotalAmount;
                    user.Role = Role.PARTICIPANT;
                }

                purchase.Status = PurchaseStatus.COMPLETED;

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
                            PurchaseId = purchase.Id,
                            RegistrationDate = DateTime.UtcNow,
                            Attendance = false
                        };

                        // First, save the participant to the database to generate the Id
                        await _context.Participants.AddAsync(participant);
                        await _context.SaveChangesAsync(); // Ensure the Id is generated

                        // Generate QR Code after participant.Id is available
                        var qrCodeData = $"{ticket.Id}_{ticket.EventId}_{participant.PurchaseId}_{participant.Id}_{Guid.NewGuid()}";

                        ticket.QRCodeData = qrCodeData;

                        // Use the ImageRepository to generate and store the QR code
                        ticket.QRCodeImageUrl = await GenerateQRCodeImage(qrCodeData)
                            ?? throw new BadRequestException("QR Code generation failed.");

                        // Send emails (outside transaction block, but still inside try-catch)
                        var ticketDto = _mapper.Map<TicketDto>(ticket);

                        try
                        {
                            var emailMessage = await _codeRepository.SendTicketToEmail(purchase.User.Email, ticketDto);
                            Console.WriteLine(emailMessage);
                        }
                        catch (Exception ex)
                        {
                            // Handle email sending failure (optional)
                            throw new BadRequestException("Email sending failed: " + ex.Message, ex.InnerException);
                        }

                        // Now, add the participant to the ticket's Participants list
                        ticket.Participants.Add(participant);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return "Ticket purchased succesfully!";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new BadRequestException(ex.Message, ex.InnerException);
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

        private async Task<string> GenerateQRCodeImage(string qrText)
        {
            try
            {
                // Generate QR code
                using QRCodeGenerator qrGenerator = new QRCodeGenerator();
                using QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
                using PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);

                // Get QR code as byte array
                byte[] qrCodeBytes = qrCode.GetGraphic(20);

                // Define the QR Codes folder inside "Uploads"
                string uploadsFolder = Path.Combine(_environment.ContentRootPath, "Uploads", "QRCodes");

                // Ensure the directory exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate unique file name for the QR code
                string fileName = $"{Guid.NewGuid()}.png";
                string filePath = Path.Combine(uploadsFolder, fileName);

                // Save QR code image
                await File.WriteAllBytesAsync(filePath, qrCodeBytes);

                // Get the request details for full URL
                var request = _httpContextAccessor.HttpContext!.Request;
                string baseUrl = $"{request.Scheme}://{request.Host}";

                // Return the complete URL
                return $"{baseUrl}/Uploads/QRCodes/{fileName}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating QR code: {ex.Message}");
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }
    }
}
