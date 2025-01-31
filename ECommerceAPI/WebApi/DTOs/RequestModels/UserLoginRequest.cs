using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.WebApi.DTOs.RequestModels
{
    public class UserLoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}