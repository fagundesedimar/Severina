using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Severina.Application.Commands.Transactions;
using Severina.Application.DTOs;
using Severina.Application.Queries.Transactions;
using Severina.Domain.Enums;

namespace Severina.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransactionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private Guid GetCompanyId() => Guid.Parse(User.FindFirstValue("CompanyId")!);

    [HttpGet]
    public async Task<ActionResult<PagedTransactionResponse>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] TransactionType? tipo = null,
        [FromQuery] TransactionCategory? categoria = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] Guid? clientId = null)
    {
        var companyId = GetCompanyId();
        var result = await _mediator.Send(new ListTransactionsQuery(companyId, page, pageSize, tipo, categoria, from, to, clientId));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TransactionResponse>> GetById(Guid id)
    {
        var companyId = GetCompanyId();
        var result = await _mediator.Send(new GetTransactionByIdQuery(companyId, id));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<TransactionResponse>> Create([FromBody] CreateTransactionRequest request)
    {
        var companyId = GetCompanyId();
        var command = new CreateTransactionCommand(
            companyId, request.Tipo, request.Valor, request.Data,
            request.Categoria, request.ClientId, request.Descricao);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TransactionResponse>> Update(Guid id, [FromBody] UpdateTransactionRequest request)
    {
        var companyId = GetCompanyId();
        var command = new UpdateTransactionCommand(
            companyId, id, request.Tipo, request.Valor, request.Data,
            request.Categoria, request.ClientId, request.Descricao);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var companyId = GetCompanyId();
        await _mediator.Send(new DeleteTransactionCommand(companyId, id));
        return NoContent();
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<ActionResult<TransactionResponse>> Approve(Guid id)
    {
        var companyId = GetCompanyId();
        var result = await _mediator.Send(new ApproveTransactionCommand(companyId, id));
        return Ok(result);
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<ActionResult<TransactionResponse>> Reject(Guid id, [FromBody] RejectTransactionRequest request)
    {
        var companyId = GetCompanyId();
        var result = await _mediator.Send(new RejectTransactionCommand(companyId, id, request.Motivo));
        return Ok(result);
    }
}
