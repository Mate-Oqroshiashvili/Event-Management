using Event_Management.Models.Enums;

namespace Event_Management.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int PurchaseId { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public TicketStatus Status { get; set; }
        public string QRCodeUrl { get; set; }

        public Event Event { get; set; }
        public User? User { get; set; }
        public Purchase? Purchase { get; set; }
        public Participant? Participant { get; set; }

        public Ticket()
        {
            
        }
    }
}
