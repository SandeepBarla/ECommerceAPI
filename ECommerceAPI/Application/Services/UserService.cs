using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Application.Models;
using AutoMapper;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.Infrastructure.Repositories.Interfaces;

namespace ECommerceAPI.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, ITokenService tokenService, IMapper mapper)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task RegisterUserAsync(User user, string password)
        {
            var existingUserEntity = await _userRepository.GetByEmailAsync(user.Email);
            if (existingUserEntity != null)
                throw new InvalidOperationException("User already exists.");
            
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            user.Role = "User"; // Default role

            await _userRepository.CreateAsync(_mapper.Map<UserEntity>(user));
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            var userEntity = await _userRepository.GetByIdAsync(userId);
            if(userEntity == null) throw new KeyNotFoundException("User not found.");
            
            return _mapper.Map<User>(userEntity);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var userEntities = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<User>>(userEntities);
        }

        public async Task<User> ValidateUserCredentialsAsync(string email, string password)
        {
            var userEntity = await _userRepository.GetByEmailAsync(email);
            if (userEntity == null || !BCrypt.Net.BCrypt.Verify(password, userEntity.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            return _mapper.Map<User>(userEntity);
        }
    }
}