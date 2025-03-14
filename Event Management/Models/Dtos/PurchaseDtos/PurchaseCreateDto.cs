using Event_Management.Models.Enums;

namespace Event_Management.Models.Dtos.PurchaseDtos
{
    public class PurchaseCreateDto
    {
        public List<int> TicketIds { get; set; } = new();
        public int UserId { get; set; }
        public int? PromoCodeId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime PurchaseDate { get; set; }
        public PurchaseStatus Status { get; set; }

        public PurchaseCreateDto()
        {
            
        }
    }
}
