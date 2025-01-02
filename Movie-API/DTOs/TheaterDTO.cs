namespace MovieBookingApp.Models.DTOs
{
    public class TheaterDTO
    {
        public int TheaterId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public int TotalSeats => Rows * Columns; // Tính số ghế trong một rạp
    }

    public class TheaterCreateDTO
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
    }
}
