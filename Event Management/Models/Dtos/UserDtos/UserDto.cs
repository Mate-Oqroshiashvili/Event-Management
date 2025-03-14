using Event_Management.Models.Dtos.OrganizerDtos;
using Event_Management.Models.Dtos.ParticipantDtos;
using Event_Management.Models.Dtos.PurchaseDtos;
using Event_Management.Models.Dtos.TicketDtos;
using Event_Management.Models.Enums;

namespace Event_Management.Models.Dtos.UserDtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? ProfilePictureUrl { get; set; } = string.Empty;
        public Role Role { get; set; }
        public UserType UserType { get; set; }
        public string? EmailVerificationCode { get; set; }
        public string? SmsVerificationCode { get; set; }
        public DateTime CodeExpiration { get; set; }
        public bool IsLoggedIn { get; set; } = false;
        public OrganizerDto Organizer { get; set; }
        public List<TicketDto> Tickets { get; set; } = new List<TicketDto>();
        public List<PurchaseDto> Purchases { get; set; } = new List<PurchaseDto>();
        public List<ParticipantDto> Participants { get; set; } = new List<ParticipantDto>();

        public UserDto()
        {
            
        }
    }
}
