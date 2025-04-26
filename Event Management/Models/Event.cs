using Event_Management.Models.Enums;

namespace Event_Management.Models
{
    public class Event
    {
        public int Id { get; set; } // Unique identifier for the event
        public string Title { get; set; } // Title of the event
        public string Description { get; set; } // Description of the event
        public DateTime StartDate { get; set; } // Start date and time of the event
        public DateTime EndDate { get; set; } // End date and time of the event
        public int Capacity { get; set; } // Maximum number of attendees for the event
        public EventStatus Status { get; set; } // Status of the event (e.g., active, canceled)
        public EventCategory Category { get; set; } // Category of the event (e.g., conference, concert)
        public int BookedStaff { get; set; } // Number of staff booked for the event
        public int LocationId { get; set; } // Location ID where the event is held
        public int OrganizerId { get; set; } // Organizer ID who is managing the event
        public List<string> Images { get; set; } // List of image URLs for the event
        public Location Location { get; set; } // Location where the event is held
        public Organizer Organizer { get; set; } // Organizer managing the event
        public IEnumerable<Ticket> Tickets { get; set; } = new List<Ticket>(); // List of tickets available for the event
        public IEnumerable<Participant> Participants { get; set; } = new List<Participant>(); // List of participants attending the event
        public List<User> SpeakersAndArtists { get; set; } = new List<User>(); // List of speakers and artists for the event
        public IEnumerable<PromoCode> PromoCodes { get; set; } = new List<PromoCode>(); // List of promo codes associated with the event
        public IEnumerable<Review> Reviews { get; set; } = new List<Review>(); // List of reviews for the event
        public IEnumerable<Comment> Comments { get; set; } = new List<Comment>(); // List of comments for the event

        public Event()
        {
            
        }
    }
}
