namespace ECommerceAPI.DTOs.ResponseModels
{
    public class CartItemResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}