using Microsoft.AspNetCore.Mvc;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.DTOs.ResponseModels;
using FluentValidation;

namespace ECommerceAPI.WebApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IValidator<UserLoginRequest> _userLoginRequestValidator;

        public AuthController(IUserService userService, ITokenService tokenService, IValidator<UserLoginRequest> userLoginRequestValidator)
        {
            _userService = userService;
            _tokenService = tokenService;
            _userLoginRequestValidator = userLoginRequestValidator;
        }

        // Login Endpoint
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest loginRequest)
        {
            await _userLoginRequestValidator.ValidateAndThrowAsync(loginRequest);
            var user = await _userService.ValidateUserCredentialsAsync(loginRequest.Email, loginRequest.Password);

            var token = _tokenService.GenerateToken(user.Id, user.Email, user.Role);
            return Ok(new AuthResponse { UserId = user.Id, Token = token, Role = user.Role });
        }
    }
}