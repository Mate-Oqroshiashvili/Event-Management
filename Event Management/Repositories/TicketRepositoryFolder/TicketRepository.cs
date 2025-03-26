using AutoMapper;
using Event_Management.Data;
using Event_Management.Exceptions;
using Event_Management.Models;
using Event_Management.Models.Dtos.TicketDtos;
using Event_Management.Models.Enums;
using Event_Management.Repositories.ImageRepositoryFolder;
using Microsoft.EntityFrameworkCore;
using QRCoder;

namespace Event_Management.Repositories.TicketRepositoryFolder
{
    public class TicketRepository : ITicketRepository
    {
        private readonly DataContext _context;
        private readonly IImageRepository _imageRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _environment;

        public TicketRepository(DataContext context, IImageRepository imageRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment environment)
        {
            _context = context;
            _imageRepository = imageRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _environment = environment;
        }

        public async Task<IEnumerable<TicketDto>> GetTicketsAsync()
        {
            var tickets = await _context.Tickets
                .Include(x => x.Participant)
                .Include(x => x.Purchase)
                .Include(x => x.User)
                .Include(x => x.Event)
                    .ThenInclude(x => x.Organizer)
                .Include(x => x.Event)
                    .ThenInclude(x => x.Location)
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
                .Where(x => x.EventId == eventId && x.Event.Status != EventStatus.DELETED && x.Status != TicketStatus.CANCELED)
                .ToListAsync();

            var ticketDtos = _mapper.Map<IEnumerable<TicketDto>>(tickets);

            return ticketDtos;
        }

        public async Task<IEnumerable<TicketDto>> GetTicketsByUserIdAsync(int userId)
        {
            try
            {
                var tickets = await _context.Tickets
                    .Include(x => x.Participant)
                    .Include(x => x.Purchase)
                    .Include(x => x.Event)
                    .Include(x => x.User)
                    .Where(x => x.User!.Id == userId)
                    .ToListAsync()
                    ?? throw new NotFoundException("User does not have any tickets!");

                var ticketDtos = _mapper.Map<IEnumerable<TicketDto>>(tickets);

                return ticketDtos;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
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
                if (ticketCreateDto == null)
                {
                    throw new BadRequestException("Information for ticket creation cannot be null.");
                }

                var ticket = _mapper.Map<Ticket>(ticketCreateDto) ?? throw new BadRequestException("Mapping failed: Ticket object is null.");

                ticket.Event = await _context.Events
                    .Include(x => x.Participants)
                        .ThenInclude(x => x.Ticket)
                    .Include(x => x.Participants)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Tickets)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Tickets)
                        .ThenInclude(x => x.Participant)
                    .Include(x => x.Tickets)
                        .ThenInclude(x => x.Purchase)
                    .Include(e => e.Location)
                        .ThenInclude(l => l.Events)
                    .Include(x => x.Location)
                    .Include(x => x.Organizer)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Organizer)
                    .Include(x => x.Organizer)
                    .Include(x => x.Location)
                    .Include(x => x.SpeakersAndArtists)
                    .Include(x => x.PromoCodes)
                    .Include(x => x.Reviews)
                    .Include(x => x.Comments)
                    .FirstOrDefaultAsync(x => x.Id == ticketCreateDto.EventId)
                    ?? throw new BadRequestException("Event not found.");

                ticket.QRCodeData = "PENDING";
                ticket.QRCodeImageUrl = "PENDING";

                if (ticket.Quantity > ticket.Event.Capacity)
                    throw new BadRequestException($"Ticket quantity should not exceed the maximum event capacity! Event's maximum capacity is {ticket.Event.Capacity}.");

                await _context.Tickets.AddAsync(ticket);
                await _context.SaveChangesAsync(); // Ensure ticket.Id is assigned

                // Generate QR Code after Id is available
                var purchaseId = ticket.PurchaseId?.ToString() ?? "N/A"; // More concise null handling
                var qrCodeData = $"{ticket.Id}-{ticket.EventId}-{purchaseId}";

                ticket.QRCodeData = qrCodeData;

                // Use the ImageRepository to handle QR code generation and storing
                ticket.QRCodeImageUrl = await GenerateQRCodeImage(qrCodeData) ?? throw new BadRequestException("QR Code generation failed.");
                Console.Write(ticket.QRCodeData);
                ticket.Status = TicketStatus.AVAILABLE;
                ticket.ExpiryDate = ticket.Event?.EndDate ?? throw new BadRequestException("Event EndDate is required.");

                await _context.SaveChangesAsync(); // Save QRCodeImageUrl

                return _mapper.Map<TicketDto>(ticket);
            }
            catch (Exception ex)
            {
                throw new BadRequestException($"Error adding ticket: {ex.Message}");
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
                throw new BadRequestException($"Error validating ticket: {ex.Message}", ex.InnerException);
            }
        }

        public async Task<bool> UpdateTicketAsync(int id, TicketUpdateDto ticketUpdateDto)
        {
            var existingTicket = await _context.Tickets
                .Include(x => x.Event)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (existingTicket == null) return false;

            if (ticketUpdateDto.Quantity > existingTicket.Event.Capacity)
                throw new BadRequestException($"Ticket quantity can not exceed the maximum event capacity! Event's maximum capacity is {existingTicket.Event.Capacity}.");

            _mapper.Map(ticketUpdateDto, existingTicket);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateTicketTypeAsync(int participantId, TicketType ticketType)
        {
            var existingParticipant = await _context.Participants
                .Include(x => x.Ticket)
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
                return string.Empty; // Return an empty string if QR code generation fails
            }
        }
    }
}
