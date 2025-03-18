using Event_Management.Models.Enums;

namespace Event_Management.Models
{
    public class PromoCode
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string PromoCodeText { get; set; }
        public int SaleAmountInPercentages { get; set; }
        public int PromoCodeAmount { get; set; }
        public PromoCodeStatus PromoCodeStatus { get; set; }
        public DateTime ExpiryDate { get; set; } = DateTime.UtcNow.AddMonths(1);

        public Event Event { get; set; }
        public IEnumerable<UsedPromoCode> UsedPromoCodes { get; set; } = new List<UsedPromoCode>();

        public PromoCode() { }
    }
}
