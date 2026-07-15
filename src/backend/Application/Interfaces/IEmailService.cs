namespace Severina.Application.Interfaces;

public interface IEmailService
{
    Task SendInviteEmailAsync(string email, string code, string baseUrl);
}
