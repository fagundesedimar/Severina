namespace Severina.Application.DTOs;

public record LoginRequest(string Email, string Senha);
public record LoginResponse(string AccessToken, DateTime ExpiresAt);
public record RefreshTokenResponse(string AccessToken, DateTime ExpiresAt);
public record UserResponse(Guid Id, string Nome, string Email, string Papel, string Status);
public record CompanyResponse(Guid Id, string Nome, string CnpjCpf, string Email, string TipoPessoa, string Status);
