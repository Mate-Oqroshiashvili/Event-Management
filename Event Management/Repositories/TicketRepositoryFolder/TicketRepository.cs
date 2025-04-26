using AutoMapper;
using Event_Management.Data;
using Event_Management.Exceptions;
using Event_Management.Models;
using Event_Management.Models.Dtos.TicketDtos;
using Event_Management.Models.Enums;
using Event_Management.Repositories.ParticipantRepositoryFolder;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using ZXing;
using ZXing.Common;
using ZXing.ImageSharp;
using ZXing.QrCode;

namespace Event_Management.Repositories.TicketRepositoryFolder
{
    public class TicketRepository : ITicketRepository
    {
        private readonly DataContext _context; // Database context for accessing the database
        private readonly IParticipantRepository _participantRepository; // Repository for participant-related operations
        private readonly IMapper _mapper; // AutoMapper for mapping between DTOs and entities

        public TicketRepository(DataContext context, IParticipantRepository participantRepository, IMapper mapper)
        {
            _context = context;
            _participantRepository = participantRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all tickets from the database.
        public async Task<IEnumerable<TicketDto>> GetTicketsAsync()
        {
            var tickets = await _context.Tickets
                .Include(t => t.Participants)
                .Include(t => t.Purchases)
                .Include(t => t.Users)
                .Include(t => t.Event)
                    .ThenInclude(e => e.Organizer)
                .Include(t => t.Event)
                    .ThenInclude(e => e.Location)
                .ToListAsync();

            var ticketDtos = _mapper.Map<IEnumerable<TicketDto>>(tickets);

            return ticketDtos;
        }

        /// <summary>
        /// Retrieves tickets associated with a specific event ID.
        public async Task<IEnumerable<TicketDto>> GetTicketsByEventIdAsync(int eventId)
        {
            var tickets = await _context.Tickets
                .Include(x => x.Participants)
                .Include(x => x.Purchases)
                .Include(x => x.Event)
                .Include(x => x.Users)
                .Where(x => x.EventId == eventId && x.Event.Status != EventStatus.DELETED && x.Status != TicketStatus.CANCELED)
                .ToListAsync();

            var ticketDtos = _mapper.Map<IEnumerable<TicketDto>>(tickets);

            return ticketDtos;
        }

        /// <summary>
        /// Retrieves tickets associated with a specific user ID.
        public async Task<IEnumerable<TicketDto>> GetTicketsByUserIdAsync(int userId)
        {
            try
            {
                var tickets = await _context.Tickets
                    .Include(x => x.Participants)
                    .Include(x => x.Purchases)
                    .Include(x => x.Event)
                    .Include(x => x.Users)
                    .Where(x => x.Users!.Any(u => u.Id == userId))
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

        /// <summary>
        /// Retrieves a ticket by its ID.
        public async Task<TicketDto> GetTicketByIdAsync(int id)
        {
            try
            {
                var ticket = await _context.Tickets
                    .Include(x => x.Participants)
                    .Include(x => x.Purchases)
                    .Include(x => x.Event)
                    .Include(x => x.Users)
                    .FirstOrDefaultAsync(x => x.Id == id) ?? throw new NotFoundException("Ticket not found!");

                var ticketDto = _mapper.Map<TicketDto>(ticket);

                return ticketDto;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Adds a new ticket to the database.
        public async Task<TicketDto> AddTicketAsync(TicketCreateDto ticketCreateDto)
        {
            if (ticketCreateDto == null)
                throw new BadRequestException("Information for ticket creation cannot be null.");

            var ticket = _mapper.Map<Ticket>(ticketCreateDto)
                ?? throw new BadRequestException("Mapping failed: Ticket object is null.");

            ticket.Event = await _context.Events
                .Include(e => e.Participants)
                .Include(e => e.Tickets)
                    .ThenInclude(x => x.Purchases)
                        .ThenInclude(x => x.Participants)
                .Include(e => e.Location)
                .Include(e => e.Organizer)
                    .ThenInclude(x => x.User)
                .Include(e => e.SpeakersAndArtists)
                .Include(e => e.PromoCodes)
                .Include(e => e.Reviews)
                .Include(e => e.Comments)
                .FirstOrDefaultAsync(e => e.Id == ticketCreateDto.EventId)
                ?? throw new BadRequestException("Event not found.");

            int threshold = 0;

            foreach (var item in ticket.Event.Tickets) // Loop through all tickets associated with the event
            {
                threshold += item.Quantity; // Add the quantity of each ticket to the threshold
                var purchase = item.Purchases.FirstOrDefault(x => x.Participants.Count != 0);

                // Check if there are any purchases with participants
                if (purchase != null)
                {
                    threshold += purchase.Participants.Count; // Add the number of participants in the purchase to the threshold
                }
            }

            if (threshold >= ticket.Event.Capacity)
                throw new BadRequestException("Tickets capacity exceeds the event's maximum capacity!");

            if ((threshold + ticketCreateDto.Quantity) > ticket.Event.Capacity)
                throw new BadRequestException("Tickets capacity exceeds the event's maximum capacity!");

            if (ticket.Event.StartDate <= DateTime.UtcNow + TimeSpan.FromHours(1) &&
                ticket.Event.StartDate > DateTime.UtcNow)
                throw new BadRequestException("You cannot add new tickets within one hour before the event starts.");

            ticket.QRCodeData = "PENDING"; // Placeholder for QR code data
            ticket.QRCodeImageUrl = "PENDING"; // Placeholder for QR code image URL

            if (ticket.Quantity > ticket.Event.Capacity)
                throw new BadRequestException($"Ticket quantity should not exceed the event's max capacity of {ticket.Event.Capacity}.");

            var existingTicket = ticket.Event.Tickets.FirstOrDefault(
                x => x.EventId == ticket.EventId && x.Price == ticket.Price && x.Type == ticket.Type
            ); // Check if a ticket with the same event, price, and type already exists

            if (existingTicket != null)
            {
                existingTicket.Quantity += ticket.Quantity; // Increase the quantity of the existing ticket

                // Check if the existing ticket is sold out and update its status
                if (existingTicket.Status == TicketStatus.SOLD_OUT) 
                    existingTicket.Status = TicketStatus.AVAILABLE; 

                ticket = existingTicket;

                await _context.SaveChangesAsync();
            }
            else
            {
                // Add the new ticket to the database
                ticket.Users.Add(ticket.Event.Organizer.User);

                await _context.Tickets.AddAsync(ticket);
                await _context.SaveChangesAsync();

                ticket.Status = TicketStatus.AVAILABLE; // Set the status to available
                ticket.ExpiryDate = ticket.Event?.EndDate
                    ?? throw new BadRequestException("Event EndDate is required.");

                await _context.SaveChangesAsync();
            }

            return _mapper.Map<TicketDto>(ticket);
        }

        /// <summary>
        /// Validates a ticket using a QR code image.
        public async Task<string> ValidateTicketByQRCodeImage(IFormFile uploadedQrCodeImage)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Decode the QR code from the image
                string qrCodeData = DecodeQRCode(uploadedQrCodeImage);

                Console.WriteLine($"Decoded QR Code Data: {qrCodeData}");

                if (string.IsNullOrEmpty(qrCodeData))
                {
                    return "Invalid QR code or image.";
                }

                // Parse the QR code data (which should be in the format TicketId_EventId_PurchaseIds_ParticipantId_RandomGuid)
                var qrParts = qrCodeData.Split('_');
                if (qrParts.Length != 5)
                {
                    return "Invalid QR code format.";
                }

                // Extract the relevant parts from the QR code data
                int ticketId = int.Parse(qrParts[0]);
                int eventId = int.Parse(qrParts[1]);
                var purchaseId = int.Parse(qrParts[2]);
                int participantId = int.Parse(qrParts[3]);
                string randomGuid = qrParts[4];

                var ticket = await _context.Tickets
                    .Include(x => x.Participants)
                    .Include(x => x.Purchases)
                    .Include(x => x.Event)
                    .FirstOrDefaultAsync(x => x.Id == ticketId);

                if (ticket == null)
                {
                    return "Ticket not found.";
                }

                if (ticket.Event.Id != eventId)
                {
                    return "Event ID mismatch.";
                }

                if (!ticket.Purchases.Any(x => x.Id == purchaseId))
                {
                    return "Invalid purchase IDs.";
                }

                // Validate the participant
                var participant = await _participantRepository.GetParticipantByIdAsync(participantId);
                if (participant == null || participant.Ticket.Id != ticketId)
                {
                    return "Invalid participant.";
                }

                if (ticket.Event.StartDate > DateTime.UtcNow)
                    throw new BadRequestException("Event has not started yet!");

                if (ticket.Event.EndDate < DateTime.UtcNow)
                    throw new BadRequestException("Event already finished!");

                if (ticket.Participants.Find(x => x.Id == participantId)!.IsUsed &&
                   ticket.Participants.Find(x => x.Id == participantId)!.Attendance)
                    throw new BadRequestException("Ticket is already validated");

                ticket.Participants.Find(x => x.Id == participantId)!.IsUsed = true;
                ticket.Participants.Find(x => x.Id == participantId)!.Attendance = true;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return "Ticket validated successfully.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Updates an existing ticket in the database.
        public async Task<string> UpdateTicketAsync(int id, TicketUpdateDto ticketUpdateDto)
        {
            try
            {
                var existingTicket = await _context.Tickets
                    .Include(x => x.Event)
                    .FirstOrDefaultAsync(t => t.Id == id) ?? throw new NotFoundException("Ticket not found!");

                int threshold = 0;

                foreach (var item in existingTicket.Event.Tickets)
                {
                    threshold += item.Quantity;
                    var purchase = item.Purchases.FirstOrDefault(x => x.Participants.Count != 0);
                    if (purchase != null)
                    {
                        threshold += purchase.Participants.Count;
                    }
                }

                if (threshold >= existingTicket.Event.Capacity)
                    throw new BadRequestException("Tickets capacity exceeds the event's maximum capacity!");

                _mapper.Map(ticketUpdateDto, existingTicket);

                await _context.SaveChangesAsync();
                return "Ticket updated successfully!";
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Deletes a ticket by its ID.
        public async Task<string> DeleteTicketAsync(int id)
        {
            try
            {
                var ticket = await _context.Tickets.FindAsync(id) ?? throw new NotFoundException("Ticket not found!");

                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
                return "Ticket deleted successfully!";
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Decodes a QR code from an image file.
        public string DecodeQRCode(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new BadRequestException("Invalid file");

                using var memoryStream = new MemoryStream();
                file.CopyTo(memoryStream);
                var byteArray = memoryStream.ToArray();

                using var image = Image.Load<Rgba32>(byteArray);
                var luminanceSource = new ImageSharpLuminanceSource<Rgba32>(image);
                var bitmap = new BinaryBitmap(new HybridBinarizer(luminanceSource));
                var reader = new QRCodeReader();
                var result = reader.decode(bitmap);

                return result?.Text ?? throw new Exception("QR code not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decoding QR code: {ex.Message}");
                throw new BadRequestException("Failed to decode QR code", ex);
            }
        }
    }
}
