namespace MovieBookingApp.Models.DTOs
{
    public class MovieDTO
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public string Duration { get; set; } // Changed to string to match the Movie model
        public DateTime ReleaseDate { get; set; } // Added for detailed movie information
        public string ImageUrl { get; set; } // Added for movie image reference
    }

    public class MovieCreateDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public string Duration { get; set; } // Changed to string to match the Movie model
        public DateTime ReleaseDate { get; set; } // Added for movie creation
        public string ImageUrl { get; set; } // Added for movie creation
    }

    public class MovieUpdateDTO
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public string Duration { get; set; } // Changed to string to match the Movie model
        public DateTime ReleaseDate { get; set; } // Added for movie update
        public string ImageUrl { get; set; } // Added for movie update
    }
}
