namespace MovieBookingApp.Models
{
    public class ItemOrder
    {
        public int ItemId { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }

        // Navigation properties
        public Item Item { get; set; }
        public Order Order { get; set; }
    }
}
