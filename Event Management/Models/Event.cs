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
        public Location Location { get; set; }
        public int Capacity { get; set; }
        public Organizer Organizer { get; set; }
        public EventStatus Status { get; set; }
        public int LocationId { get; set; }
        public int OrganizerId { get; set; }

        public IEnumerable<Ticket> Tickets { get; set; } = new List<Ticket>();
        public IEnumerable<Participant> Participants { get; set; } = new List<Participant>();

        public Event()
        {
            
        }
    }
}
