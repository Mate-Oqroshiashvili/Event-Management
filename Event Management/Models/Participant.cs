namespace Event_Management.Models
{
    public class Participant
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
        public int TicketId { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public bool Attendance { get; set; } = false;

        public Event Event { get; set; }
        public Ticket Ticket { get; set; }
        public User User { get; set; }

        public Participant()
        {
            
        }
    }
}
