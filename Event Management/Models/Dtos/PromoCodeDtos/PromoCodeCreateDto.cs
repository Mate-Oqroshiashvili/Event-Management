namespace Event_Management.Models.Dtos.PromoCodeDtos
{
    public class PromoCodeCreateDto
    {
        public int EventId { get; set; }
        public string PromoCodeText { get; set; }
        public int SaleAmountInPercentages { get; set; }
        public int PromoCodeAmount { get; set; }

        public PromoCodeCreateDto()
        {
            
        }
    }
}
