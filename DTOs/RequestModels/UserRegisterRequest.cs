using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTOs.RequestModels
{
    public class UserRegisterRequest
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}