namespace MovieBookingApp.Models.DTOs
{
    public class RevenueReportDTO
    {
        public int MovieId { get; set; }
        public string MovieTitle { get; set; }
        public DateTime ReportDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalTicketsSold { get; set; }
    }
}
