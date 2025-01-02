namespace MovieBookingApp.Models.DTOs
{
    public class MovieDTO
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public string Duration { get; set; } // chuyển đổi Duration thành string
        public DateTime ReleaseDate { get; set; } // Thêm thông tin ngày phát hành
        public string ImageUrl { get; set; }
    }

    public class MovieCreateDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public string Duration { get; set; } 
        public DateTime ReleaseDate { get; set; } 
        public string ImageUrl { get; set; } 
    }

    public class MovieUpdateDTO
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public string Duration { get; set; } 
        public DateTime ReleaseDate { get; set; } 
        public string ImageUrl { get; set; } 
    }
}
