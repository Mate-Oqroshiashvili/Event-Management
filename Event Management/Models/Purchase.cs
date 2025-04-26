using Event_Management.Models.Enums;

namespace Event_Management.Models
{
    public class Purchase
    {
        public int Id { get; set; } // Unique identifier for the purchase
        public int UserId { get; set; } // User ID of the person who made the purchase
        public int? PromoCodeId { get; set; } // Promo code ID used for the purchase
        public decimal TotalAmount { get; set; } // Total amount of the purchase
        public DateTime PurchaseDate { get; set; } // Date when the purchase was made
        public PurchaseStatus Status { get; set; } // Status of the purchase (e.g., completed, pending, refunded)
        public bool isPromoCodeUsed { get; set; } = false; // Track if promo code was used

        public User User { get; set; } // User associated with the purchase
        public PromoCode PromoCode { get; set; } // Promo code associated with the purchase
        public List<Ticket> Tickets { get; set; } = new(); // List of tickets associated with the purchase
        public List<Participant> Participants { get; set; } = new(); // List of participants associated with the purchase

        public Purchase()
        {
            
        }
    }
}
