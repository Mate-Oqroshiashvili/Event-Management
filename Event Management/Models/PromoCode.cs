using Event_Management.Models.Enums;

namespace Event_Management.Models
{
    public class PromoCode
    {
        public int Id { get; set; } // Unique identifier for the promo code
        public int EventId { get; set; } // Event ID to which the promo code belongs
        public string PromoCodeText { get; set; } // The actual promo code text
        public int SaleAmountInPercentages { get; set; } // Discount amount in percentages
        public int PromoCodeAmount { get; set; } // Total number of times the promo code can be used
        public PromoCodeStatus PromoCodeStatus { get; set; } // Status of the promo code (e.g., active, expired)
        public DateTime ExpiryDate { get; set; } = DateTime.UtcNow.AddMonths(1); // Expiry date of the promo code

        public Event Event { get; set; } // Event associated with the promo code
        public IEnumerable<UsedPromoCode> UsedPromoCodes { get; set; } = new List<UsedPromoCode>(); // List of used promo codes associated with this promo code

        public PromoCode() { }
    }
}
