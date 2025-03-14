using Event_Management.Models.Dtos.TicketDtos;

namespace Event_Management.Repositories.CodeRepositoryFolder
{
    public interface ICodeRepository
    {
        Task<string> SendToEmail(string email, string text);
        Task<string> SendToPhone(string phone, string text);
        Task<string> SendTicketToEmail(string email, TicketDto ticket);
    }
}
