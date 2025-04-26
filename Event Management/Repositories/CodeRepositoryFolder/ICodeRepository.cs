using Event_Management.Models.Dtos.TicketDtos;

namespace Event_Management.Repositories.CodeRepositoryFolder
{
    public interface ICodeRepository
    {
        Task<string> SendToEmail(string email, string text); // Sends a message to the specified email address
        Task<string> SendToPhone(string phone, string text); // Sends a message to the specified phone number
        Task<string> SendTicketToEmail(string email, TicketDto ticket); // Sends a ticket to the specified email address
        Task SendEventCancellationNotification(string email, string eventTitle); // Sends a cancellation notification for an event to the specified email address
        Task SendEventRescheduleNotification(string email, string eventTitle, DateTime newDate); // Sends a reschedule notification for an event to the specified email address
        Task<string> SendCodes(int userId); // Sends a verification code to the specified user ID
        Task<bool> SendCodes(string email, string phoneNumber); // Sends a verification code to the specified email and phone number
        string GetCodes(string email); // Retrieves the verification codes associated with the specified email address
    }
}
