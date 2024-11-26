namespace MovieBookingApp.Models
{
    public class Ticket
    {
        public int TicketId { get; set; }
        public int ShowtimeId { get; set; }
        public int SeatId { get; set; }
        public decimal Price { get; set; }
        public int UserId { get; set; }
		public int MovieId { get; set; }
        public Movie Movie { get; set; }
		public int TheaterId { get; set; }
        public Theater  Theater { get; set; }
        // Navigation Properties
        public Showtime Showtime { get; set; }
        public Seat Seat { get; set; }
        public ICollection<TicketInfo> TicketInfos { get; set; }
    }
}
