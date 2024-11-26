namespace MovieBookingApp.Models
{
    public class Theater
    {
        public int TheaterId { get; set; }
        public string Name { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public string? Location { get; set; }

		public ICollection<Showtime> Showtimes { get; set; }
    }
}
