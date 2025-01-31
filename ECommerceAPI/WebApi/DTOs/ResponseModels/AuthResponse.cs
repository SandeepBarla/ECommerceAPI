namespace ECommerceAPI.WebApi.DTOs.ResponseModels
{
    public class AuthResponse
    {
        public int UserId { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
    }
}