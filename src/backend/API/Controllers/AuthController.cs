using Microsoft.AspNetCore.Mvc;
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

    public AuthController(
        IUserRepository userRepository,
        IPasswordService passwordService,
        ITokenService tokenService,
        ILogger<AuthController> logger)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _logger = logger;
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

        Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7),
            Path = "/api/v1/auth"
        });

        return Ok(new LoginResponse(accessToken, DateTime.UtcNow.AddMinutes(15)));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<RefreshTokenResponse>> Refresh()
    {
        var refreshToken = Request.Cookies["refresh_token"];

        if (string.IsNullOrEmpty(refreshToken) || !_tokenService.ValidateRefreshToken(refreshToken))
        {
            return Unauthorized(new { message = "Refresh token inválido" });
        }

        return Ok(new RefreshTokenResponse("new_access_token", DateTime.UtcNow.AddMinutes(15)));
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
}
