using Event_Management.Models.Dtos.CommentDtos;
using Event_Management.Models.Dtos.OrganizerDtos;
using Event_Management.Models.Dtos.ParticipantDtos;
using Event_Management.Models.Dtos.PromoCodeDtos;
using Event_Management.Models.Dtos.PurchaseDtos;
using Event_Management.Models.Dtos.ReviewDtos;
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
        public decimal Balance { get; set; }
        public string? EmailVerificationCode { get; set; }
        public string? SmsVerificationCode { get; set; }
        public DateTime CodeExpiration { get; set; }
        public bool IsLoggedIn { get; set; } = false;
        public OrganizerDto Organizer { get; set; }
        public IEnumerable<TicketDto> Tickets { get; set; }
        public IEnumerable<PurchaseDto> Purchases { get; set; }
        public IEnumerable<ParticipantDto> Participants { get; set; }
        public IEnumerable<ReviewDto> Reviews { get; set; }
        public IEnumerable<CommentDto> Comments { get; set; }
        public IEnumerable<UsedPromoCodeDto> UsedPromoCodes { get; set; }

        public UserDto()
        {

        }
    }
}
