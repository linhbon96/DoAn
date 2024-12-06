namespace MovieBookingApp.Models.DTOs
{
    public class TicketInfoDTO
    {
        public int TicketInfoId { get; set; }
        public int TicketId { get; set; }
        public int? OrderId { get; set; }
        public int? UserId { get; set; }
        public int SeatId { get; set; }
        public string Row { get; set; }
        public int Number { get; set; }
        public string UserName { get; set; }
        public string TicketDetails { get; set; }
        public string MovieTitle { get; set; }
        public string Genre { get; set; }
        public DateTime? ReleaseDate { get; set; }
    }

    public class TicketInfoCreateDTO
    {
        public int TicketId { get; set; }
        public int? OrderId { get; set; }
        public int? UserId { get; set; }
        public int SeatId { get; set; }
    }
}
