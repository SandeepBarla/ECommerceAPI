namespace ECommerceAPI.Application.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; } = "Pending";
        public string OrderStatus { get; set; } = "Processing";
        public DateTime OrderDate { get; set; }
        public string ShippingAddress { get; set; }
        public string TrackingNumber { get; set; } = "Not Assigned";
    }
}