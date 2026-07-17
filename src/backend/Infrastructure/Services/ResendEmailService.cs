using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resend;

namespace Severina.Infrastructure.Services;

public class ResendEmailOptions
{
    public string ApiKey { get; set; } = "";
    public string FromEmail { get; set; } = "";
    public string FromName { get; set; } = "Severina AI";
}

public class ResendEmailService : Application.Interfaces.IEmailService
{
    private readonly ResendClient _resend;
    private readonly ResendEmailOptions _options;
    private readonly ILogger<ResendEmailService> _logger;

    public ResendEmailService(
        ResendClient resend,
        IOptions<ResendEmailOptions> options,
        ILogger<ResendEmailService> logger)
    {
        _resend = resend;
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendInviteEmailAsync(string email, string code, string baseUrl)
    {
        var inviteLink = $"{baseUrl}/convite/{code}";

        var htmlBody = $"""
            <!DOCTYPE html>
            <html>
            <head><meta charset="utf-8"></head>
            <body style="font-family: Arial, sans-serif; max-width: 500px; margin: 0 auto; padding: 20px;">
                <div style="background: #004d40; color: white; padding: 20px; border-radius: 8px 8px 0 0; text-align: center;">
                    <h1 style="margin: 0; font-size: 20px;">Severina AI</h1>
                </div>
                <div style="border: 1px solid #e0e0e0; border-top: none; padding: 30px; border-radius: 0 0 8px 8px;">
                    <h2 style="color: #333; font-size: 18px;">Você foi convidado!</h2>
                    <p style="color: #666; font-size: 14px; line-height: 1.6;">
                        Um administrador convidou você para acessar a plataforma Severina AI.
                        Clique no botão abaixo para criar sua conta:
                    </p>
                    <div style="text-align: center; margin: 30px 0;">
                        <a href="{inviteLink}"
                           style="background: #004d40; color: white; padding: 12px 32px; border-radius: 6px; text-decoration: none; font-weight: bold; font-size: 14px;">
                            Aceitar Convite
                        </a>
                    </div>
                    <p style="color: #999; font-size: 12px; text-align: center;">
                        Este link expira em 7 dias.
                    </p>
                </div>
            </body>
            </html>
            """;

        var message = new EmailMessage
        {
            From = $"{_options.FromName} <{_options.FromEmail}>",
            To = new EmailAddressList { email },
            Subject = "Convite para Severina AI",
            HtmlBody = htmlBody,
        };

        try
        {
            var result = await _resend.EmailSendAsync(message);
            _logger.LogInformation("Email de convite enviado para {Email}, id: {Id}", email, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao enviar email de convite para {Email}", email);
        }
    }
}
