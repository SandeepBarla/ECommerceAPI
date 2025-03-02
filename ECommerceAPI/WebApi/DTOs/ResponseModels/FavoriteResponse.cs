namespace ECommerceAPI.WebApi.DTOs.ResponseModels
{
    public class FavoriteResponse
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string PrimaryImageUrl { get; set; }
    }
}