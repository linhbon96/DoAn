namespace MovieBookingApp.Models.DTOs
{
    public class ShowtimeDTO
    {
        public int ShowtimeId { get; set; }
        public int MovieId { get; set; }
        public string MovieTitle { get; set; }
        public int TheaterId { get; set; }
        public string TheaterName { get; set; }
        public DateTime ShowDate { get; set; } // Chỉ lưu ngày
        public TimeSpan ShowHour { get; set; } // Chỉ lưu giờ
         public List<SeatDTO> Seats { get; set; } = new List<SeatDTO>();
    }

    public class ShowtimeCreateDTO
    {
        public int MovieId { get; set; }
        public int TheaterId { get; set; }
        public DateTime ShowDate { get; set; } // Ngày của buổi chiếu
        public TimeSpan ShowHour { get; set; } // Giờ chiếu của buổi chiếu
    }
}
