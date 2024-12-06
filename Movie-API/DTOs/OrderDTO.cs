namespace MovieBookingApp.Models.DTOs
{
    // DTO cho thông tin chi tiết của từng Item trong Order
    public class ItemOrderDTO
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } // Tên sản phẩm
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }

    // DTO cho việc tạo ItemOrder khi tạo Order mới
    public class ItemOrderCreateDTO
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public int OrderId { get; set; }
    }

    // DTO cho phản hồi của ItemOrder
    public class ItemOrderResponseDTO
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } // Tên sản phẩm
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }

    // DTO cho việc tạo Order mới
    public class OrderCreateDTO
{
    public int UserId { get; set; } 
    public List<TicketCreateDTO> Tickets { get; set; }
    public List<ItemOrderCreateDTO> ItemOrders { get; set; } = new List<ItemOrderCreateDTO>();
}


    // DTO cho phản hồi của Order
    public class OrderResponseDTO
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<ItemOrderResponseDTO> ItemOrders { get; set; }
    }

    // DTO đầy đủ của Order (nếu cần cả thông tin chi tiết và sửa đổi)
    public class OrderDTO
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<ItemOrderDTO> ItemOrders { get; set; }
    }
}
