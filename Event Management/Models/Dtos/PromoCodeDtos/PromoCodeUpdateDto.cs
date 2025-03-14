using Event_Management.Models.Enums;

namespace Event_Management.Models.Dtos.PromoCodeDtos
{
    public class PromoCodeUpdateDto
    {
        public int? PromoCodeAmount { get; set; }
        public PromoCodeStatus? PromoCodeStatus { get; set; }

        public PromoCodeUpdateDto() { }
    }
}
