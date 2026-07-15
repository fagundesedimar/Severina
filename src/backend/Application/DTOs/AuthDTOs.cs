namespace Severina.Application.DTOs;

public record LoginRequest(string Email, string Senha);
public record LoginResponse(string AccessToken, DateTime ExpiresAt);
public record RefreshTokenResponse(string AccessToken, DateTime ExpiresAt);
