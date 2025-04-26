using Event_Management.Models;
using Event_Management.Models.Dtos.TicketDtos;
using Event_Management.Models.Enums;

namespace Event_Management.Repositories.TicketRepositoryFolder
{
    public interface ITicketRepository
    {
        Task<IEnumerable<TicketDto>> GetTicketsAsync(); // Retrieves all tickets
        Task<IEnumerable<TicketDto>> GetTicketsByEventIdAsync(int eventId); // Retrieves tickets by event ID 
        Task<IEnumerable<TicketDto>> GetTicketsByUserIdAsync(int userId); // Retrieves tickets by user ID
        Task<TicketDto> GetTicketByIdAsync(int id); // Retrieves a ticket by its ID
        Task<TicketDto> AddTicketAsync(TicketCreateDto ticketCreateDto); // Adds a new ticket
        Task<string> ValidateTicketByQRCodeImage(IFormFile uploadedQrCodeImage); // Validates a ticket by its QR code image
        Task<string> UpdateTicketAsync(int id, TicketUpdateDto ticketUpdateDto); // Updates an existing ticket
        Task<string> DeleteTicketAsync(int id); // Deletes a ticket by its ID
    }
}
