using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Severina.Application.Commands.Transactions;
using Severina.Application.Commands.Invoices;
using Severina.Application.Interfaces;
using Severina.Domain.Enums;

namespace Severina.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ExportController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITenantProvider _tenantProvider;

    public ExportController(IMediator mediator, ITenantProvider tenantProvider)
    {
        _mediator = mediator;
        _tenantProvider = tenantProvider;
    }

    private Guid CompanyId => _tenantProvider.CompanyId ?? Guid.Parse(User.FindFirstValue("company_id")!);

    [HttpPost("transactions")]
    [EnableRateLimiting("export")]
    public async Task<IActionResult> ExportTransactions(
        [FromQuery] string format = "csv",
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] TransactionType? tipo = null,
        [FromQuery] TransactionCategory? categoria = null,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new ExportTransactionsCommand(CompanyId, format, from, to, tipo, categoria), ct);

        return File(result.FileBytes, result.ContentType, result.FileName);
    }

    [HttpPost("invoices")]
    [EnableRateLimiting("export")]
    public async Task<IActionResult> ExportInvoices(
        [FromQuery] string format = "csv",
        [FromQuery] InvoiceStatus? status = null,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new ExportInvoicesCommand(CompanyId, format, status), ct);

        return File(result.FileBytes, result.ContentType, result.FileName);
    }
}
