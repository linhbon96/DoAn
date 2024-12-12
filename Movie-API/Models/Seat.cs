namespace MovieBookingApp.Models
{
    public class Seat
    {
        public int Id { get; set; }
        public string Row { get; set; }
        public int Number { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsLocked => LockedUntil.HasValue && LockedUntil > DateTime.UtcNow;
        public DateTime? LockedUntil { get; set; }
        public int ShowTimeId { get; set; }
        public Showtime ShowTime { get; set; }

		public int? OrderId { get; set; }
		public Order Order { get; set; }
    }
}
