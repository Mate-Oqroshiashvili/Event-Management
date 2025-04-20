namespace Event_Management.Models.Dtos.EventDtos
{
    public class EventAnalyticsDto
    {
        public int EventId { get; set; }
        public string Title { get; set; }

        // Attendance
        public int TotalParticipants { get; set; }
        public int AttendedParticipants { get; set; }
        public double OccupancyRate { get; set; }

        // Tickets by Type
        public int VIPTicketCount { get; set; }
        public int BasicTicketCount { get; set; }
        public int EarlyBirdTicketCount { get; set; }
        public int TotalTicketQuantity { get; set; }

        // Financials
        public int TotalTicketsSold { get; set; }
        public decimal TotalRevenue { get; set; }

        // Promo Codes
        public int PromoCodesUsed { get; set; }
        public int PurchasesWithPromoCode { get; set; }
        public int PurchasesWithoutPromoCode { get; set; }
        public int AvailablePromoCodes { get; set; }
        public int OutOfStockPromoCodes { get; set; }
        public int TotalPromoCodes { get; set; }

        // Reviews & Comments
        public int ReviewCount { get; set; }
        public double AverageRating { get; set; }
        public int CommentCount { get; set; }

        public int FiveStarCount { get; set; }
        public int FourStarCount { get; set; }
        public int ThreeStarCount { get; set; }
        public int TwoStarCount { get; set; }
        public int OneStarCount { get; set; }
    }
}
