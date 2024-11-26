namespace MovieBookingApp.Models.DTOs
{
    public class TicketInfoDTO
    {
        public int TicketInfoId { get; set; }
        public int TicketId { get; set; }
        public int? OrderId { get; set; }
        public int? UserId { get; set; }

        // Các thông tin chi tiết khác nếu cần
        public string UserName { get; set; } // Nếu cần hiển thị tên người dùng
        public string TicketDetails { get; set; } // Nếu cần hiển thị chi tiết vé
    }

    public class TicketInfoCreateDTO
    {
        public int TicketId { get; set; }
        public int? OrderId { get; set; }
        public int? UserId { get; set; }
    }
}
