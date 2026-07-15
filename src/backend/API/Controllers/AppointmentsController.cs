using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Severina.Application.Commands.Appointments;
using Severina.Application.DTOs;
using Severina.Application.Queries.Appointments;
using Severina.Domain.Enums;

namespace Severina.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AppointmentsController> _logger;

    public AppointmentsController(IMediator mediator, ILogger<AppointmentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<PagedAppointmentResponse>> List(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] Guid? clientId,
        [FromQuery] StatusCompromisso? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var companyId = GetCompanyId();
        var result = await _mediator.Send(new ListAppointmentsQuery(companyId, from, to, clientId, status, page, pageSize));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AppointmentResponse>> GetById(Guid id)
    {
        var companyId = GetCompanyId();
        var result = await _mediator.Send(new GetAppointmentByIdQuery(companyId, id));

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentResponse>> Create([FromBody] CreateAppointmentRequest request)
    {
        var companyId = GetCompanyId();
        var command = new CreateAppointmentCommand(
            companyId, request.Titulo, request.Descricao,
            request.DataHoraInicio, request.DataHoraFim,
            request.Tipo, request.ClientId);

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AppointmentResponse>> Update(Guid id, [FromBody] UpdateAppointmentRequest request)
    {
        var companyId = GetCompanyId();
        var command = new UpdateAppointmentCommand(
            companyId, id, request.Titulo, request.Descricao,
            request.DataHoraInicio, request.DataHoraFim,
            request.Tipo, request.ClientId);

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var companyId = GetCompanyId();
        await _mediator.Send(new CancelAppointmentCommand(companyId, id));
        return Ok(new { message = "Appointment cancelado com sucesso" });
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id)
    {
        var companyId = GetCompanyId();
        await _mediator.Send(new CompleteAppointmentCommand(companyId, id));
        return Ok(new { message = "Appointment concluído com sucesso" });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var companyId = GetCompanyId();
        await _mediator.Send(new DeleteAppointmentCommand(companyId, id));
        return NoContent();
    }

    private Guid GetCompanyId()
    {
        var claim = User.FindFirst("company_id")?.Value;
        return Guid.TryParse(claim, out var companyId) ? companyId : Guid.Empty;
    }
}
