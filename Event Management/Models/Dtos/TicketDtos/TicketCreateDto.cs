using Event_Management.Models.Enums;

namespace Event_Management.Models.Dtos.TicketDtos
{
    public class TicketCreateDto
    {
        public int EventId { get; set; }
        public TicketType Type { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public TicketCreateDto()
        {
            
        }
    }
}
