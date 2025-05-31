using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerceAPI.Application.Models;
using ECommerceAPI.Infrastructure.Entities;

namespace ECommerceAPI.Infrastructure.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task CreateAsync(UserEntity userEntity);
        Task<UserEntity?> GetByIdAsync(int userId);
        Task<UserEntity?> GetByEmailAsync(string email);
        Task<IEnumerable<UserEntity>> GetAllAsync();
        Task UpdateAsync(UserEntity userEntity);
    }
}