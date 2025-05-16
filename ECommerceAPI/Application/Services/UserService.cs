using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Application.Models;
using AutoMapper;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.Infrastructure.Repositories.Interfaces;
using Google.Apis.Auth;

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

        public async Task<User> RegisterUserAsync(User user, string password)
        {
            var existingUserEntity = await _userRepository.GetByEmailAsync(user.Email);
            if (existingUserEntity != null)
                throw new InvalidOperationException("User already exists.");
            
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            user.Role = "User"; // Default role
            var userEntity = _mapper.Map<UserEntity>(user);
            await _userRepository.CreateAsync(userEntity);
            return _mapper.Map<User>(userEntity);
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
        
        public async Task<User> FindOrCreateUserFromGoogleAsync(GoogleJsonWebSignature.Payload payload)
        {
            // Try to find the user by email
            var existingUserEntity = await _userRepository.GetByEmailAsync(payload.Email);

            if (existingUserEntity != null)
            {
                // User already exists → login
                return _mapper.Map<User>(existingUserEntity);
            }

            // New user → register them using Google profile
            var newUser = new User
            {
                Email = payload.Email,
                FullName = payload.Name,
                Role = "User",                   // Default role for new users
                PasswordHash = null             // No password since Google handles authentication
            };

            // Convert domain model to entity
            var newUserEntity = _mapper.Map<UserEntity>(newUser);

            // Save to database
            await _userRepository.CreateAsync(newUserEntity);

            // Return the created user as domain model
            return _mapper.Map<User>(newUserEntity);
        }
    }
}