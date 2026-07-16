using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Severina.Application.DTOs;
using Severina.Application.Interfaces;
using Severina.Domain.Interfaces;

namespace Severina.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthController> _logger;
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _configuration;

    public AuthController(
        IUserRepository userRepository,
        IPasswordService passwordService,
        ITokenService tokenService,
        ILogger<AuthController> logger,
        IWebHostEnvironment env,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _logger = logger;
        _env = env;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null || !_passwordService.VerifyPassword(request.Senha, user.SenhaHash))
        {
            return Unauthorized(new { message = "Credenciais inválidas" });
        }

        var accessToken = _tokenService.GenerateAccessToken(
            user.Id,
            user.Email,
            user.CompanyId,
            user.Papel.ToString()
        );

        var refreshToken = _tokenService.GenerateRefreshToken();
        AppendRefreshTokenCookie(refreshToken);

        return Ok(new LoginResponse(accessToken, DateTime.UtcNow.AddMinutes(15)));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<RefreshTokenResponse>> Refresh()
    {
        var authHeader = Request.Headers.Authorization.FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            return Unauthorized(new { message = "Refresh token inválido" });
        }

        var expiredToken = authHeader["Bearer ".Length..].Trim();

        var principal = GetPrincipalFromExpiredToken(expiredToken);
        if (principal == null)
        {
            return Unauthorized(new { message = "Refresh token inválido" });
        }

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Refresh token inválido" });
        }

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return Unauthorized(new { message = "Refresh token inválido" });
        }

        var newAccessToken = _tokenService.GenerateAccessToken(
            user.Id,
            user.Email,
            user.CompanyId,
            user.Papel.ToString()
        );

        var newRefreshToken = _tokenService.GenerateRefreshToken();
        AppendRefreshTokenCookie(newRefreshToken);

        return Ok(new RefreshTokenResponse(newAccessToken, DateTime.UtcNow.AddMinutes(15)));
    }

    [HttpPost("logout")]
    public ActionResult Logout()
    {
        Response.Cookies.Delete("refresh_token", new CookieOptions
        {
            Path = "/api/v1/auth"
        });

        return Ok(new { message = "Logout realizado com sucesso" });
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    private void AppendRefreshTokenCookie(string refreshToken)
    {
        Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = !_env.IsDevelopment(),
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(7),
            Path = "/api/v1/auth"
        });
    }
}
