namespace Event_Management.Models.Dtos.UserDtos
{
    public class AdminAnalyticsDto
    {
        public int TotalUsers { get; set; }
        public int TotalOrganizers { get; set; }
        public int TotalParticipants { get; set; }
        public int TotalArtists { get; set; }
        public int TotalSpeakers { get; set; }

        public int TotalEvents { get; set; }
        public int DraftedEvents { get; set; }
        public int PublishedEvents { get; set; }
        public int CompletedEvents { get; set; }
        public int DeletedEvents { get; set; }

        public decimal TotalRevenue { get; set; }
        public decimal TotalUserBalances { get; set; }

        public int TotalComments { get; set; }
        public int TotalReviews { get; set; }
        public int TotalPromoCodes { get; set; }
    }
}
