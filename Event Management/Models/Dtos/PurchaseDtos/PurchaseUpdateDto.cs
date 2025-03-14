using Event_Management.Models.Enums;

namespace Event_Management.Models.Dtos.PurchaseDtos
{
    public class PurchaseUpdateDto
    {
        public int? Quantity { get; set; }
        public PurchaseStatus? Status { get; set; }
        public List<int>? TicketIds { get; set; }

        public PurchaseUpdateDto()
        {
            
        }
    }
}
