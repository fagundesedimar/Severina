using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Severina.Application.Commands.Invoices;
using Severina.Application.DTOs;
using Severina.Application.Queries.Invoices;
using Severina.Domain.Enums;

namespace Severina.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class InvoicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public InvoicesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private Guid GetCompanyId() => Guid.Parse(User.FindFirstValue("CompanyId")!);

    [HttpGet]
    public async Task<ActionResult<PagedInvoiceResponse>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] InvoiceStatus? status = null,
        [FromQuery] Guid? clientId = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var companyId = GetCompanyId();
        var result = await _mediator.Send(new ListInvoicesQuery(companyId, page, pageSize, status, clientId, from, to));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<InvoiceResponse>> GetById(Guid id)
    {
        var companyId = GetCompanyId();
        var result = await _mediator.Send(new GetInvoiceByIdQuery(companyId, id));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<InvoiceResponse>> Create([FromBody] CreateInvoiceRequest request)
    {
        var companyId = GetCompanyId();
        var command = new CreateInvoiceCommand(
            companyId, request.Valor, request.DataVencimento,
            request.ClientId, request.Descricao);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<InvoiceResponse>> Update(Guid id, [FromBody] UpdateInvoiceRequest request)
    {
        var companyId = GetCompanyId();
        var command = new UpdateInvoiceCommand(
            companyId, id, request.Valor, request.DataVencimento,
            request.ClientId, request.Descricao);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var companyId = GetCompanyId();
        await _mediator.Send(new DeleteInvoiceCommand(companyId, id));
        return NoContent();
    }

    [HttpPost("{id:guid}/pay")]
    public async Task<ActionResult<InvoiceResponse>> Pay(Guid id, [FromBody] PayInvoiceRequest request)
    {
        var companyId = GetCompanyId();
        var result = await _mediator.Send(new PayInvoiceCommand(companyId, id, request.ValorPago, request.DataPagamento));
        return Ok(result);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var companyId = GetCompanyId();
        await _mediator.Send(new CancelInvoiceCommand(companyId, id));
        return NoContent();
    }
}
