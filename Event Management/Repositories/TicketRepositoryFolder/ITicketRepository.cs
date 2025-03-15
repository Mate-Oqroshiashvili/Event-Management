using Event_Management.Models;
using Event_Management.Models.Dtos.TicketDtos;

namespace Event_Management.Repositories.TicketRepositoryFolder
{
    public interface ITicketRepository
    {
        Task<IEnumerable<TicketDto>> GetTicketsAsync();
        Task<IEnumerable<TicketDto>> GetTicketsByEventIdAsync(int eventId);
        Task<TicketDto> GetTicketByIdAsync(int id);
        Task<TicketDto> AddTicketAsync(TicketCreateDto ticketCreateDto);
        Task<string> ValidateTicketAsync(int ticketId, string qrCodeData);
        Task<bool> UpdateTicketAsync(int id, TicketUpdateDto ticketUpdateDto);
        Task<bool> DeleteTicketAsync(int id);
    }
}
