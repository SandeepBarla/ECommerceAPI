namespace ECommerceAPI.Application.Models
{
    public class User
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; } // Hashed password

        public string Role { get; set; } = "Customer"; // Default role to "Customer"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Make Orders nullable (optional)
        public ICollection<Order>? Orders { get; set; } = new List<Order>();
    }
}