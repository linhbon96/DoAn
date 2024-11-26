namespace MovieBookingApp.Models
{
    public class TicketInfo
    {
        public int TicketInfoId { get; set; }
        public int TicketId { get; set; }
        public int? OrderId { get; set; }
        public int? UserId { get; set; }

        // Navigation properties
        public Ticket Ticket { get; set; }
        public Order Order { get; set; }
        public User User { get; set; }
    }
}