namespace Event_Management.Models
{
    public class Participant
    {
        public int Id { get; set; } // Unique identifier for the participant
        public int EventId { get; set; } // Event ID to which the participant belongs
        public int UserId { get; set; } // User ID of the participant
        public int? TicketId { get; set; } // Ticket ID associated with the participant
        public int? PurchaseId { get; set; } // Purchase ID associated with the participant
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow; // Date of registration
        public bool Attendance { get; set; } = false; // Track if participant attended the event
        public bool IsUsed { get; set; } = false; // Track if QR is scanned

        public Event Event { get; set; } // Event associated with the participant
        public Ticket Ticket { get; set; } // Ticket associated with the participant
        public Purchase Purchase { get; set; } // Purchase associated with the participant
        public User User { get; set; } // User associated with the participant

        public Participant()
        {
            
        }
    }
}
