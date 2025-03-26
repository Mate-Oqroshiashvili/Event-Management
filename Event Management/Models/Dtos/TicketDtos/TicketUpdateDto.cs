using Event_Management.Models.Enums;

namespace Event_Management.Models.Dtos.TicketDtos
{
    public class TicketUpdateDto
    {
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public TicketStatus? Status { get; set; }

        public TicketUpdateDto()
        {
            
        }
    }
}
