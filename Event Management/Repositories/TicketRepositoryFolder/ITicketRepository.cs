using Event_Management.Models;
using Event_Management.Models.Dtos.TicketDtos;
using Event_Management.Models.Enums;

namespace Event_Management.Repositories.TicketRepositoryFolder
{
    public interface ITicketRepository
    {
        Task<IEnumerable<TicketDto>> GetTicketsAsync();
        Task<IEnumerable<TicketDto>> GetTicketsByEventIdAsync(int eventId);
        Task<IEnumerable<TicketDto>> GetTicketsByUserIdAsync(int userId);
        Task<TicketDto> GetTicketByIdAsync(int id);
        Task<TicketDto> AddTicketAsync(TicketCreateDto ticketCreateDto);
        Task<string> ValidateTicketAsync(int ticketId, string qrCodeData);
        Task<bool> UpdateTicketAsync(int id, TicketUpdateDto ticketUpdateDto);
        Task<bool> UpdateTicketTypeAsync(int participantId, TicketType ticketType);
        Task<bool> DeleteTicketAsync(int id);
    }
}
