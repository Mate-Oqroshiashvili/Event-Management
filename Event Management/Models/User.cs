using Event_Management.Models.Enums;

namespace Event_Management.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public string? ProfilePicture { get; set; } = string.Empty;
        public Role Role { get; set; }
        public UserType UserType { get; set; }
        public string? EmailVerificationCode { get; set; }
        public string? SmsVerificationCode { get; set; }
        public DateTime CodeExpiration { get; set; }
        public bool IsLoggedIn { get; set; } = false;

        public Organizer? Organizer { get; set; }


        // Navigation Properties
        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
        public List<Purchase> Purchases { get; set; } = new List<Purchase>();
        public List<Participant> Participants { get; set; } = new List<Participant>();

        public User()
        {
            
        }
    }
}
