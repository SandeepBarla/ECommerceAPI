namespace ECommerceAPI.DTOs.ResponseModels
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public string Products { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public string ShippingAddress { get; set; }
        public string TrackingNumber { get; set; }
    }
}