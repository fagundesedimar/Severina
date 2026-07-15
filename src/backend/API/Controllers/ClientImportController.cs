using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Severina.Application.Commands.Clients;
using Severina.Application.DTOs;

namespace Severina.API.Controllers;

[ApiController]
[Route("api/v1/clients/import")]
[Authorize]
[EnableRateLimiting("import")]
public class ClientImportController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ClientImportController> _logger;
    private const long MaxFileSize = 10 * 1024 * 1024; // 10MB
    private static readonly string[] AllowedExtensions = { ".csv", ".xlsx" };

    public ClientImportController(IMediator mediator, ILogger<ClientImportController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ImportJobResponse>> Import(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "Arquivo vazio" });

        if (file.Length > MaxFileSize)
            return BadRequest(new { message = "Arquivo excede limite de 10MB" });

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            return BadRequest(new { message = "Formato não suportado. Use CSV ou XLSX" });

        var companyId = GetCompanyId();

        using var stream = file.OpenReadStream();
        var command = new ImportClientsCommand(companyId, stream, file.FileName);
        var result = await _mediator.Send(command);

        return Accepted(result);
    }

    private Guid GetCompanyId()
    {
        var claim = User.FindFirst("company_id")?.Value;
        return Guid.TryParse(claim, out var companyId) ? companyId : Guid.Empty;
    }
}
