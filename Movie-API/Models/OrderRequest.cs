namespace MovieBookingApp.Controllers;

public class OrderRequest
{
    public int ShowtimeId { get; set; }
    public List<int> SeatIds { get; set; }
    public int UserId { get; set; }
    public decimal TotalAmount { get; set; }
    // Các trường khác của order
}