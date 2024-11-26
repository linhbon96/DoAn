namespace MovieBookingApp.Models
{
   public class Showtime
{
    public int ShowtimeId { get; set; }
    public int MovieId { get; set; }
    public int TheaterId { get; set; }
    public DateTime ShowDate { get; set; } // Chỉ lưu ngày
    public TimeSpan ShowHour { get; set; } // Chỉ lưu giờ
    public Theater Theater { get; set; }
    public Movie Movie { get; set; }
     public ICollection<Seat> Seats { get; set; }
}
}