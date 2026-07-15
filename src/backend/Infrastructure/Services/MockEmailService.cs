using Microsoft.Extensions.Logging;

namespace Severina.Infrastructure.Services;

public class MockEmailService : Application.Interfaces.IEmailService
{
    private readonly ILogger<MockEmailService> _logger;

    public MockEmailService(ILogger<MockEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendInviteEmailAsync(string email, string code, string baseUrl)
    {
        var inviteLink = $"{baseUrl}/convite/{code}";
        _logger.LogInformation("MOCK EMAIL: Enviando convite para {Email} com link: {Link}", email, inviteLink);
        return Task.CompletedTask;
    }
}
