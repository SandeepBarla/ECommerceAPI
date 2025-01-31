using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerceAPI.Application.Models;
using ECommerceAPI.WebApi.DTOs.RequestModels;

namespace ECommerceAPI.Application.Interfaces
{
    public interface IUserService
    {
        Task<User> RegisterUserAsync(User user, string password);
        Task<User> GetUserByIdAsync(int userId);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> ValidateUserCredentialsAsync(string email, string password);
    }
}