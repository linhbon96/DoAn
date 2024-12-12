namespace MovieBookingApp.Models.DTOs
{
    public class SeatDTO
    {
        public int SeatId { get; set; }
        public string Row { get; set; } 
        public int Number { get; set; } 
        public bool IsAvailable { get; set; }
        public bool IsLocked { get; set; } 
        public DateTime? LockedUntil { get; set; }
		public int? OrderId { get; set; }
        public int ShowTimeId { get; set; } 
        public ShowtimeDTO ShowTime { get; set; }
    }
    public class SeatCheckDTO
    {
        public int ShowtimeId { get; set; }
        public List<int> SeatIds { get; set; }
    }

}
