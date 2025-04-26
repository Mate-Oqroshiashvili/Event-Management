namespace Event_Management.Models
{
    public class Review
    {
        public int Id { get; set; } // Unique identifier for the review
        public int StarCount { get; set; } // Number of stars given in the review
        public int UserId { get; set; } // User ID of the person who made the review
        public User User { get; set; } // User associated with the review
        public int EventId { get; set; } // Event ID to which the review belongs
        public Event Event { get; set; } // Event associated with the review
    }
}
