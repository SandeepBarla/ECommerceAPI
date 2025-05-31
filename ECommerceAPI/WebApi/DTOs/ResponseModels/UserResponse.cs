namespace ECommerceAPI.WebApi.DTOs.ResponseModels
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string Role { get; set; }
        public ICollection<AddressResponse>? Addresses { get; set; } = new List<AddressResponse>();
    }
}