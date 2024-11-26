namespace MovieBookingApp.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }  // Lưu trữ mật khẩu đã băm
        public string Role { get; set; }  // e.g., "Admin" or "User"
        public ICollection<TicketInfo> TicketInfos { get; set; }
    }

    public class UserRole
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
