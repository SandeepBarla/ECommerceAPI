using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.WebApi.DTOs.RequestModels
{
    public class UserRegisterRequest
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}