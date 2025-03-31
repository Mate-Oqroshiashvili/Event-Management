namespace Event_Management.Models.Dtos.PromoCodeDtos
{
    public class UsedPromoCodeDto
    {
        public int Id { get; set; }
        public int PromoCodeId { get; set; }
        public int UserId { get; set; }
        public DateTime UsedDate { get; set; }
    }
}
