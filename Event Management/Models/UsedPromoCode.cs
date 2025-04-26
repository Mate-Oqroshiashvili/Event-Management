namespace Event_Management.Models
{
    public class UsedPromoCode
    {
        public int Id { get; set; } // Unique identifier for the used promo code
        public int PromoCodeId { get; set; } // Promo code ID that was used
        public int UserId { get; set; } // User ID of the person who used the promo code
        public DateTime UsedDate { get; set; } // Date when the promo code was used

        public PromoCode PromoCode { get; set; } // Promo code associated with the used promo code
        public User User { get; set; } // User associated with the used promo code
    }
}
