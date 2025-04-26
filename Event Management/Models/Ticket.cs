using Event_Management.Models.Enums;

namespace Event_Management.Models
{
    public class Ticket
    {
        public int Id { get; set; } // Unique identifier for the ticket
        public int EventId { get; set; } // Event ID to which the ticket belongs
        public int? PurchaseId { get; set; } // Purchase ID associated with the ticket
        public TicketType Type { get; set; } // Type of the ticket (e.g., VIP, Regular)
        public decimal Price { get; set; } // Price of the ticket
        public int Quantity { get; set; } // Quantity of tickets available
        public TicketStatus Status { get; set; } // Status of the ticket (e.g., available, sold out)
        public string QRCodeData { get; set; }  // QR Code String
        public string QRCodeImageUrl { get; set; }  // Image URL
        public DateTime ExpiryDate { get; set; } // QR Expiration date

        public Event Event { get; set; } // Event associated with the ticket
        public List<User> Users { get; set; } = new List<User>(); // List of users associated with the ticket
        public List<Purchase> Purchases { get; set; } = new List<Purchase>(); // List of purchases associated with the ticket
        public List<Participant> Participants { get; set; } = new List<Participant>(); // List of participants associated with the ticket

        public Ticket()
        {
            
        }
    }
}
