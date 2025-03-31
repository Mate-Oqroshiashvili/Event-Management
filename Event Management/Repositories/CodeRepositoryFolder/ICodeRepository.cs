using Event_Management.Models.Dtos.TicketDtos;

namespace Event_Management.Repositories.CodeRepositoryFolder
{
    public interface ICodeRepository
    {
        Task<string> SendToEmail(string email, string text);
        Task<string> SendToPhone(string phone, string text);
        Task<string> SendTicketToEmail(string email, TicketDto ticket);
        Task SendEventCancellationNotification(string email, string eventTitle);
        Task SendEventRescheduleNotification(string email, string eventTitle, DateTime newDate);
        Task<string> SendCodes(int userId);
        Task<bool> SendCodes(string email, string phoneNumber);
        string GetCodes(string email);
    }
}
