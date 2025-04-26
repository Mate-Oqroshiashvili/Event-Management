namespace Event_Management.Models
{
    public class Organizer
    {
        public int Id { get; set; }  // Primary Key
        public string Name { get; set; }  // Organizer's Name (Company or Individual)
        public string Email { get; set; }  // Contact Email
        public string PhoneNumber { get; set; }  // Contact Phone Number
        public string Description { get; set; }  // Short Bio or Details about the Organizer
        public string LogoUrl { get; set; }  // URL for Organizer's Logo/Image
        public string Address { get; set; }  // Office Address (Optional)
        public string City { get; set; }  // City (Optional)
        public string Country { get; set; }  // Country (Optional)
        public bool IsVerified { get; set; } = false;  // Flag to indicate if the organizer is verified
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Timestamp when the organizer was created
        public int UserId { get; set; } // User ID of the organizer (if applicable)
        public User User { get; set; } // User associated with the organizer (if applicable)

        // Navigation Property
        public List<Event> Events { get; set; } = new List<Event>();  // List of events organized by this entity
        public List<Location> Locations { get; set; } = new List<Location>(); // Direct Many-to-Many

        public Organizer()
        {
            
        }
    }
}
