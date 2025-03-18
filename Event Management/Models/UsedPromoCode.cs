namespace Event_Management.Models
{
    public class UsedPromoCode
    {
        public int Id { get; set; }
        public int PromoCodeId { get; set; }
        public int UserId { get; set; }
        public DateTime UsedDate { get; set; }

        public PromoCode PromoCode { get; set; }
        public User User { get; set; }
    }
}
