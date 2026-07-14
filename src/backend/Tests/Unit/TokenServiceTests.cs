using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Severina.Infrastructure.Services;
using Xunit;

namespace Severina.Tests.Unit;

public class TokenServiceTests
{
    private readonly TokenService _tokenService;

    public TokenServiceTests()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "TestKey123456789012345678901234567890",
                ["Jwt:Issuer"] = "Severina",
                ["Jwt:Audience"] = "SeverinaApp"
            })
            .Build();

        _tokenService = new TokenService(config);
    }

    [Fact]
    public void GenerateAccessToken_ReturnsValidToken()
    {
        var userId = Guid.NewGuid();
        var token = _tokenService.GenerateAccessToken(userId, "test@test.com", Guid.NewGuid(), "Administrador");

        Assert.False(string.IsNullOrEmpty(token));

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        Assert.Equal("Severina", jwtToken.Issuer);
        Assert.Equal("SeverinaApp", jwtToken.Audiences.First());
    }

    [Fact]
    public void GenerateAccessToken_ContainsCorrectClaims()
    {
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var token = _tokenService.GenerateAccessToken(userId, "test@test.com", companyId, "Operacional");

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.NameIdentifier && c.Value == userId.ToString());
        Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Email && c.Value == "test@test.com");
        Assert.Contains(jwtToken.Claims, c => c.Type == "company_id" && c.Value == companyId.ToString());
        Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Operacional");
    }

    [Fact]
    public void GenerateAccessToken_HasCorrectExpiry()
    {
        var token = _tokenService.GenerateAccessToken(Guid.NewGuid(), "test@test.com", Guid.NewGuid(), "Admin");
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var expectedExpiry = DateTime.UtcNow.AddMinutes(15);
        var actualExpiry = jwtToken.ValidTo;

        Assert.True(Math.Abs((expectedExpiry - actualExpiry).TotalSeconds) < 5);
    }

    [Fact]
    public void GenerateRefreshToken_ReturnsNonEmptyString()
    {
        var token = _tokenService.GenerateRefreshToken();
        Assert.False(string.IsNullOrEmpty(token));
        Assert.Equal(88, token.Length);
    }

    [Fact]
    public void GenerateRefreshToken_DifferentTokens()
    {
        var token1 = _tokenService.GenerateRefreshToken();
        var token2 = _tokenService.GenerateRefreshToken();
        Assert.NotEqual(token1, token2);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ValidateRefreshToken_EmptyToken_ReturnsFalse(string? token)
    {
        Assert.False(_tokenService.ValidateRefreshToken(token!));
    }

    [Fact]
    public void ValidateRefreshToken_ValidToken_ReturnsTrue()
    {
        var token = _tokenService.GenerateRefreshToken();
        Assert.True(_tokenService.ValidateRefreshToken(token));
    }

    [Fact]
    public void ValidateRefreshToken_WrongLength_ReturnsFalse()
    {
        Assert.False(_tokenService.ValidateRefreshToken("short"));
        Assert.False(_tokenService.ValidateRefreshToken(new string('a', 87)));
        Assert.False(_tokenService.ValidateRefreshToken(new string('a', 89)));
    }
}
