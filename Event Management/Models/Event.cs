using Event_Management.Models.Enums;

namespace Event_Management.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Capacity { get; set; }
        public EventStatus Status { get; set; }
        public EventCategory Category { get; set; }
        public int BookedStaff { get; set; }
        public int LocationId { get; set; }
        public int OrganizerId { get; set; }
        public List<string> Images { get; set; }  
        public Location Location { get; set; } 
        public Organizer Organizer { get; set; }
        public IEnumerable<Ticket> Tickets { get; set; } = new List<Ticket>();
        public IEnumerable<Participant> Participants { get; set; } = new List<Participant>();
        public List<User> SpeakersAndArtists { get; set; } = new List<User>();
        public IEnumerable<PromoCode> PromoCodes { get; set; } = new List<PromoCode>();
        public IEnumerable<Review> Reviews { get; set; } = new List<Review>();
        public IEnumerable<Comment> Comments { get; set; } = new List<Comment>();

        public Event()
        {
            
        }
    }
}
