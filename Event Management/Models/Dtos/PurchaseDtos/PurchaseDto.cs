using Event_Management.Models.Dtos.TicketDtos;
using Event_Management.Models.Dtos.UserDtos;
using Event_Management.Models.Enums;

namespace Event_Management.Models.Dtos.PurchaseDtos
{
    public class PurchaseDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime PurchaseDate { get; set; }
        public PurchaseStatus Status { get; set; }
        public UserDto User { get; set; }
        public PromoCode? PromoCode { get; set; }
        public List<TicketDto> Tickets { get; set; } = new();
    }
}
