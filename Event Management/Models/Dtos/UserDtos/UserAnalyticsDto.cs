namespace Event_Management.Models.Dtos.UserDtos
{
    public class UserAnalyticsDto
    {
        public decimal TotalBalance { get; set; }
        public decimal TotalSpent { get; set; }
        public int TotalPurchases { get; set; }
        public int TotalTicketsBought { get; set; }

        public int EventsParticipatedIn { get; set; } // From attended tickets
        public int EventsAsArtistOrSpeaker { get; set; }

        public int TotalComments { get; set; }
        public int TotalReviews { get; set; }

        public int UsedPromoCodesCount { get; set; }
    }
}
