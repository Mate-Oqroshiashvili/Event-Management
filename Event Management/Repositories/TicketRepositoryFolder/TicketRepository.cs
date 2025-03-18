using AutoMapper;
using Event_Management.Data;
using Event_Management.Models;
using Event_Management.Models.Dtos.TicketDtos;
using Event_Management.Models.Enums;
using Event_Management.Repositories.ImageRepositoryFolder;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using SkiaSharp.QrCode;

namespace Event_Management.Repositories.TicketRepositoryFolder
{
    public class TicketRepository : ITicketRepository
    {
        private readonly DataContext _context;
        private readonly IImageRepository _imageRepository;
        private readonly IMapper _mapper;

        public TicketRepository(DataContext context, IImageRepository imageRepository, IMapper mapper)
        {
            _context = context;
            _imageRepository = imageRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TicketDto>> GetTicketsAsync()
        {
            var tickets = await _context.Tickets
                .Include(x => x.Participant)
                .Include(x => x.Purchase)
                .Include(x => x.Event)
                .Include(x => x.User)
                .ToListAsync();

            var ticketDtos = _mapper.Map<IEnumerable<TicketDto>>(tickets);

            return ticketDtos;
        }

        public async Task<IEnumerable<TicketDto>> GetTicketsByEventIdAsync(int eventId)
        {
            var tickets = await _context.Tickets
                .Include(x => x.Participant)
                .Include(x => x.Purchase)
                .Include(x => x.Event)
                .Include(x => x.User)
                .Where(x => x.EventId == eventId)
                .ToListAsync();

            var ticketDtos = _mapper.Map<IEnumerable<TicketDto>>(tickets);

            return ticketDtos;
        }

        public async Task<TicketDto> GetTicketByIdAsync(int id)
        {
            var ticket = await _context.Tickets
                .Include(x => x.Participant)
                .Include(x => x.Purchase)
                .Include(x => x.Event)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            var ticketDto = _mapper.Map<TicketDto>(ticket);

            return ticketDto;
        }

        public async Task<TicketDto> AddTicketAsync(TicketCreateDto ticketCreateDto)
        {
            try
            {
                var ticket = _mapper.Map<Ticket>(ticketCreateDto);
                await _context.Tickets.AddAsync(ticket);
                await _context.SaveChangesAsync(); // Ensure ticket.Id is assigned

                // Generate QR Code after Id is available
                var purchaseId = ticket.PurchaseId?.ToString() ?? "N/A"; // More concise null handling
                var qrCodeData = $"{ticket.Id}-{ticket.EventId}-{purchaseId}";

                ticket.QRCodeData = qrCodeData;

                // Use the ImageRepository to handle QR code generation and storing
                ticket.QRCodeImageUrl = await GenerateQRCodeImage(qrCodeData); // Async call
                ticket.Status = TicketStatus.AVAILABLE;
                ticket.ExpiryDate = ticket.Event.EndDate;

                await _context.SaveChangesAsync(); // Save QRCodeImageUrl

                return _mapper.Map<TicketDto>(ticket);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding ticket: {ex.Message}");
                return null!; // Return null or handle the error appropriately
            }
        }

        public async Task<string> ValidateTicketAsync(int ticketId, string qrCodeData)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var ticket = await _context.Tickets
                .Include(x => x.Participant)
                .Include(x => x.Purchase)
                .Include(x => x.Event)
                .Include(x => x.User)
                .FirstOrDefaultAsync(t => t.Id == ticketId && t.QRCodeData == qrCodeData);

                if (ticket == null)
                    return "Invalid ticket"; // Specific error if ticket not found

                if (ticket.IsUsed)
                    return "Ticket has already been used"; // Error if ticket already used

                if (ticket.User == null)
                    return "The owner of this ticket can't be identified";

                if (ticket.Participant == null)
                    return "The owner of this ticket is not registered as participant";

                // Mark ticket as used
                ticket.IsUsed = true;
                ticket.Participant.Attendance = true;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return "Ticket validated successfully"; // Clear success message
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error validating ticket: {ex.Message}");
                return "An error occurred while validating the ticket";
            }
        }

        public async Task<bool> UpdateTicketAsync(int id, TicketUpdateDto ticketUpdateDto)
        {
            var existingTicket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);
            if (existingTicket == null) return false;

            _mapper.Map(ticketUpdateDto, existingTicket);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateTicketTypeAsync(int participantId, TicketType ticketType)
        {
            var existingParticipant = await _context.Participants
                .Include(x => x.Event)
                .Include(x => x.Ticket)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == participantId);

            if (existingParticipant == null || existingParticipant.Ticket == null)
                return false;

            existingParticipant.Ticket.Type = ticketType;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTicketAsync(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return false;

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<string> GenerateQRCodeImage(string qrText)
        {
            try
            {
                int qrSize = 256; // QR code image size

                // Generate QR Code data
                var generator = new QRCodeGenerator();
                var qrData = generator.CreateQrCode(qrText, ECCLevel.Q);

                // Create QR Code Bitmap
                using (var bitmap = new SKBitmap(qrSize, qrSize))
                using (var canvas = new SKCanvas(bitmap))
                {
                    canvas.Clear(SKColors.White);

                    // Define the renderer
                    var renderer = new QRCodeRenderer();
                    var qrRect = new SKRect(0, 0, qrSize, qrSize);
                    renderer.Render(canvas, qrRect, qrData, SKColors.White, SKColors.Black, null);

                    // Convert to image
                    using (var image = SKImage.FromBitmap(bitmap))
                    using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                    {
                        string tempFilePath = Path.GetTempFileName();
                        await File.WriteAllBytesAsync(tempFilePath, data.ToArray());

                        // Use ImageRepository to handle file storage
                        using var stream = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read);
                        var formFile = new FormFile(stream, 0, stream.Length, "qrCode", "qrcode.png");

                        return await _imageRepository.GenerateImageSource(formFile);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating QR code: {ex.Message}");
                return string.Empty; // Return an empty string if QR code generation fails
            }
        }
    }
}
