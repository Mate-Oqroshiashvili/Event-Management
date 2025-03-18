using Event_Management.Models.Enums;

namespace Event_Management.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? PromoCodeId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime PurchaseDate { get; set; }
        public PurchaseStatus Status { get; set; }
        public bool isPromoCodeUsed { get; set; } = false;

        public User User { get; set; }
        public PromoCode PromoCode { get; set; }
        public List<Ticket> Tickets { get; set; } = new();

        public Purchase()
        {
            
        }
    }
}
