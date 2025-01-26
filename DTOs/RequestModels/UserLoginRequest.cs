using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTOs.RequestModels
{
    public class UserLoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}