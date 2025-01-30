using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using ECommerceAPI.Application.Models;
using ECommerceAPI.Application.Services;

public class TokenServiceTests
{
    private readonly TokenService _tokenService;

    public TokenServiceTests()
    {
        // Mock configuration for JWT settings
        var inMemorySettings = new Dictionary<string, string>
        {
            { "Jwt:Key", "VerySecretKeyForTestingOnly12345!" },  // Must be at least 16 chars
            { "Jwt:Issuer", "TestIssuer" },
            { "Jwt:Audience", "TestAudience" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _tokenService = new TokenService(configuration);
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidJwtToken_WhenUserIsValid()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@example.com", Role = "Customer" };

        // Act
        var token = _tokenService.GenerateToken(user.Id, user.Email, user.Role);

        // Assert
        Assert.NotNull(token);
        Assert.False(string.IsNullOrEmpty(token));

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        Assert.Equal("TestIssuer", jwtToken.Issuer);
        Assert.Equal("TestAudience", jwtToken.Audiences.First());
        Assert.Contains(jwtToken.Claims, c => c.Type == "nameid" && c.Value == user.Id.ToString());
        Assert.Contains(jwtToken.Claims, c => c.Type == "email" && c.Value == user.Email);
        Assert.Contains(jwtToken.Claims, c => c.Type == "role" && c.Value == user.Role);
    }
    

    [Fact]
    public void GenerateToken_ShouldThrowException_WhenJwtKeyIsInvalid()
    {
        // Arrange
        var badConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Jwt:Key", "ShortKey" },  // Invalid because it's too short
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" }
            })
            .Build();

        var badTokenService = new TokenService(badConfig);
        var user = new User { Id = 1, Email = "test@example.com", Role = "Customer" };

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => badTokenService.GenerateToken(user.Id, user.Email, user.Role));
    }
}