namespace MovieBookingApp.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public ICollection<Seat> Seats { get; set; }
        public decimal TotalAmount { get; set; }

        public ICollection<ItemOrder> ItemOrders { get; set; }
        public ICollection<TicketInfo> TicketInfos { get; set; }
    }

}