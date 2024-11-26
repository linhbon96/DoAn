namespace MovieBookingApp.Models
{
    public class Movie
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Genre { get; set; }
        public string Duration { get; set; }
        public string ImageUrl { get; set; }
        public List<Showtime> Showtimes { get; set; }
    }

}