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
        Task<string> ValidateTicketByQRCodeImage(IFormFile uploadedQrCodeImage);
        Task<string> UpdateTicketAsync(int id, TicketUpdateDto ticketUpdateDto);
        Task<string> DeleteTicketAsync(int id);
    }
}
