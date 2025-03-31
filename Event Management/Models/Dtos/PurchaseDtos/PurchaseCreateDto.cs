using Event_Management.Models.Dtos.TicketDtos;

namespace Event_Management.Models.Dtos.PurchaseDtos
{
    public class PurchaseCreateDto
    {
        public List<TicketPurchaseRequest> Tickets { get; set; } = new();
        public int UserId { get; set; }
        public string? PromoCodeText { get; set; }

        public PurchaseCreateDto()
        {

        }
    }
}
