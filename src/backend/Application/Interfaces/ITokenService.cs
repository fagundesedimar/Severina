namespace Severina.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(Guid userId, string email, Guid companyId, string papel);
    string GenerateRefreshToken();
    bool ValidateRefreshToken(string token);
}
