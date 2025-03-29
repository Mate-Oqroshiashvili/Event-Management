using Event_Management.Models.Enums;

namespace Event_Management.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int? PurchaseId { get; set; }
        public TicketType Type { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public TicketStatus Status { get; set; }
        public string QRCodeData { get; set; }  // QR Code String
        public string QRCodeImageUrl { get; set; }  // Image URL
        public DateTime ExpiryDate { get; set; } // QR Expiration date

        public Event Event { get; set; }
        public List<User> Users { get; set; } = new List<User>();
        public List<Purchase> Purchases { get; set; } = new List<Purchase>();
        public List<Participant> Participants { get; set; } = new List<Participant>();

        public Ticket()
        {
            
        }
    }
}
