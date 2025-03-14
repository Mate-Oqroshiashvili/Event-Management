using Event_Management.Models.Enums;

namespace Event_Management.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int? PurchaseId { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public TicketStatus Status { get; set; }
        public string QRCodeData { get; set; }  // QR Code String
        public string QRCodeImageUrl { get; set; }  // Image URL
        public bool IsUsed { get; set; } = false; // Track if QR is scanned
        public DateTime ExpiryDate { get; set; } // QR Expiration date

        public Event Event { get; set; }
        public User? User { get; set; }
        public Purchase? Purchase { get; set; }
        public Participant? Participant { get; set; }

        public Ticket()
        {
            
        }
    }
}
