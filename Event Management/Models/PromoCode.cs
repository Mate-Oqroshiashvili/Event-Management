using Event_Management.Models.Enums;

namespace Event_Management.Models
{
    public class PromoCode
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string PromoCodeText { get; set; }
        public int PromoCodeAmount { get; set; }
        public PromoCodeStatus PromoCodeStatus { get; set; }

        public Event Event { get; set; }

        public PromoCode() { }
    }
}
