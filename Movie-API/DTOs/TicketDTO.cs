namespace MovieBookingApp.Models.DTOs
{
    public class TicketDTO
    {
        public int TicketId { get; set; }
        public int ShowtimeId { get; set; }
        public string MovieTitle { get; set; }
        public DateTime ShowDate { get; set; }
        public TimeSpan ShowHour { get; set; }
        public int SeatId { get; set; }
        public string Row { get; set; }
        public int Number { get; set; }
        public decimal Price { get; set; }
        public int MovieId { get; set; }
        public int TheaterId { get; set; }
    }

    public class TicketCreateDTO
    {
        public int ShowtimeId { get; set; }
        public int SeatId { get; set; }
        public decimal Price { get; set; }
        public int UserId { get; set; }
		public int MovieId { get; set; }
		public int TheaterId { get; set; }
    }
}
