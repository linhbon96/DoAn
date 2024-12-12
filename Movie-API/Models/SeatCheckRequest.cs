namespace MovieBookingApp.Controllers;

public class SeatCheckRequest
{
    public int ShowtimeId { get; set; }
    public List<int> SeatIds { get; set; }
}

// 
