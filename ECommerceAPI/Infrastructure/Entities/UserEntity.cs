namespace ECommerceAPI.Infrastructure.Entities
{
    public class UserEntity
    {
        public int Id { get; set; } // Primary Key
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? Phone { get; set; } // Added phone number
        public string Role { get; set; } = "Customer";

        // Navigation Properties
        public ICollection<OrderEntity> Orders { get; set; }
        public CartEntity? Cart { get; set; }

        // ✅ User's Favorite Products
        public ICollection<FavoriteEntity> Favorites { get; set; } = new List<FavoriteEntity>();

        // ✅ User's Addresses
        public ICollection<AddressEntity> Addresses { get; set; } = new List<AddressEntity>();
    }
}