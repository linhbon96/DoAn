namespace MovieBookingApp.Models
{
    public class OrderInfo
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TicketId { get; set; }
        public int Quantity { get; set; }
    }
}