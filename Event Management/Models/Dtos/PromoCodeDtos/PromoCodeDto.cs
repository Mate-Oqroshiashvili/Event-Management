using Event_Management.Models.Enums;

namespace Event_Management.Models.Dtos.PromoCodeDtos
{
    public class PromoCodeDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string PromoCodeText { get; set; }
        public PromoCodeStatus PromoCodeStatus { get; set; }
    }
}
