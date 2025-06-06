using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.DTOs.ResponseModels;
using AutoMapper;
using ECommerceAPI.Application.Models;
using FluentValidation;

namespace ECommerceAPI.WebApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IValidator<UserRegisterRequest> _userRegisterRequestValidator;
        private readonly IValidator<UserUpdateRequest> _userUpdateRequestValidator;

        public UserController(IUserService userService, ITokenService tokenService, IMapper mapper,
            IValidator<UserRegisterRequest> userRegisterRequestValidator,
            IValidator<UserUpdateRequest> userUpdateRequestValidator)
        {
            _userService = userService;
            _tokenService = tokenService;
            _mapper = mapper;
            _userRegisterRequestValidator = userRegisterRequestValidator;
            _userUpdateRequestValidator = userUpdateRequestValidator;
        }

        // Register New User
        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegisterRequest request)
        {
            await _userRegisterRequestValidator.ValidateAndThrowAsync(request);
            var user = _mapper.Map<User>(request);
            user = await _userService.RegisterUserAsync(user, request.Password);
            var token = _tokenService.GenerateToken(user.Id, user.Email, user.Role);
            var authResponse = new AuthResponse { UserId = user.Id, Role = user.Role, Token = token };
            return CreatedAtAction(nameof(GetUserById), new { userId = user.Id }, authResponse);
        }

        // Get User Profile (Authenticated Users)
        [Authorize]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            return Ok(_mapper.Map<UserResponse>(user));
        }

        // Update User Profile (Authenticated Users)
        [Authorize]
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUserProfile(int userId, [FromBody] UserUpdateRequest request)
        {
            await _userUpdateRequestValidator.ValidateAndThrowAsync(request);
            var updatedUser = await _userService.UpdateUserProfileAsync(userId, request.FullName, request.Phone);
            return Ok(_mapper.Map<UserResponse>(updatedUser));
        }

        // Get All Users (Admin Only)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(_mapper.Map<IEnumerable<UserResponse>>(users));
        }
    }
}