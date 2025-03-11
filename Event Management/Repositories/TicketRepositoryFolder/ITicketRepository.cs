using Event_Management.Models;
using Event_Management.Models.Dtos.TicketDtos;

namespace Event_Management.Repositories.TicketRepositoryFolder
{
    public interface ITicketRepository
    {
        Task<IEnumerable<TicketDto>> GetTicketsAsync();
        Task<TicketDto> GetTicketByIdAsync(int id);
        Task<TicketDto> AddTicketAsync(TicketCreateDto ticketCreateDto);
        Task<bool> UpdateTicketAsync(int id, TicketUpdateDto ticketUpdateDto);
        Task<bool> DeleteTicketAsync(int id);
    }
}
