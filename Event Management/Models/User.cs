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
        public decimal Balance { get; set; } = 0;
        public Role Role { get; set; }
        public UserType UserType { get; set; }
        public string? EmailVerificationCode { get; set; }
        public string? SmsVerificationCode { get; set; }
        public DateTime CodeExpiration { get; set; }
        public bool IsLoggedIn { get; set; } = false;

        public Organizer? Organizer { get; set; }

        // Navigation Properties
        public IEnumerable<Ticket> Tickets { get; set; } = new List<Ticket>();
        public IEnumerable<Purchase> Purchases { get; set; } = new List<Purchase>();
        public IEnumerable<Participant> Participants { get; set; } = new List<Participant>();
        public IEnumerable<Review> Reviews { get; set; } = new List<Review>();
        public IEnumerable<Comment> Comments { get; set; } = new List<Comment>();
        public List<UsedPromoCode> UsedPromoCodes { get; set; } = new List<UsedPromoCode>();

        public User()
        {
            
        }
    }
}
