using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceAPI.Application.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; } // Hashed password

        public string Role { get; set; } = "Customer"; // Default role to "Customer"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ✅ Make Orders nullable (optional)
        public ICollection<Order>? Orders { get; set; } = new List<Order>();
    }
}