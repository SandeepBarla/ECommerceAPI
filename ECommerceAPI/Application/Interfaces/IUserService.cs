using ECommerceAPI.Application.Models;
using Google.Apis.Auth;

namespace ECommerceAPI.Application.Interfaces
{
    public interface IUserService
    {
        Task<User> RegisterUserAsync(User user, string password);
        Task<User> GetUserByIdAsync(int userId);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> ValidateUserCredentialsAsync(string email, string password);
        Task<User> FindOrCreateUserFromGoogleAsync(GoogleJsonWebSignature.Payload payload);
    }
}