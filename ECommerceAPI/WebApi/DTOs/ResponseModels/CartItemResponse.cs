namespace ECommerceAPI.WebApi.DTOs.ResponseModels
{
    public class CartItemResponse
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public ProductListResponse Product { get; set; }
        public int Quantity { get; set; }
    }
}