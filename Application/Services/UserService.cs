using AutoMapper;
using ECommerceAPI.Infrastructure.Context;
using ECommerceAPI.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Application.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
    }

    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UserService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var userEntities = await _context.Users.ToListAsync();
            return _mapper.Map<IEnumerable<User>>(userEntities);
        }
    }
}