using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Severina.Application.Commands.Appointments;
using Severina.Application.DTOs;
using Severina.Application.Queries.Appointments;

namespace Severina.API.Controllers;

[ApiController]
[Route("api/v1/recurrence")]
[Authorize]
public class RecurrenceController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<RecurrenceController> _logger;

    public RecurrenceController(IMediator mediator, ILogger<RecurrenceController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentResponse>> CreateRecurring([FromBody] CreateRecurringAppointmentRequest request)
    {
        var companyId = GetCompanyId();
        var command = new CreateRecurringAppointmentCommand(
            companyId, request.Titulo, request.Descricao,
            request.DataHoraInicio, request.DataHoraFim,
            request.Tipo, request.ClientId,
            request.RecurrenceTipo, request.RecurrenceIntervalo,
            request.RecurrenceDiasDaSemana, request.RecurrenceFimTipo,
            request.RecurrenceDataFim, request.RecurrenceContagemFim);

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetInstances), new { serieId = result.SerieId }, result);
    }

    [HttpGet("instances/{serieId:guid}")]
    public async Task<ActionResult<List<AppointmentResponse>>> GetInstances(
        Guid serieId,
        [FromQuery] DateTime from,
        [FromQuery] DateTime to)
    {
        var companyId = GetCompanyId();
        var result = await _mediator.Send(new GetRecurrenceInstancesQuery(companyId, serieId, from, to));
        return Ok(result);
    }

    [HttpPut("instance/{instanceId:guid}")]
    public async Task<ActionResult<AppointmentResponse>> EditSingleInstance(
        Guid instanceId,
        [FromBody] EditSingleInstanceRequest request)
    {
        var companyId = GetCompanyId();
        var command = new EditSingleInstanceCommand(
            companyId, instanceId, request.Titulo, request.Descricao,
            request.DataHoraInicio, request.DataHoraFim,
            request.Tipo, request.ClientId);

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("series/{serieId:guid}")]
    public async Task<IActionResult> EditSeries(
        Guid serieId,
        [FromBody] EditSeriesRequest request)
    {
        var companyId = GetCompanyId();
        var command = new EditSeriesCommand(
            companyId, serieId, request.Titulo, request.Descricao,
            request.DataHoraInicio, request.DataHoraFim,
            request.Tipo, request.ClientId,
            request.RecurrenceTipo, request.RecurrenceIntervalo,
            request.RecurrenceDiasDaSemana, request.RecurrenceFimTipo,
            request.RecurrenceDataFim, request.RecurrenceContagemFim);

        await _mediator.Send(command);
        return Ok(new { message = "Série atualizada com sucesso" });
    }

    [HttpPost("instance/{instanceId:guid}/cancel")]
    public async Task<IActionResult> CancelSingleInstance(Guid instanceId)
    {
        var companyId = GetCompanyId();
        await _mediator.Send(new CancelSingleInstanceCommand(companyId, instanceId));
        return Ok(new { message = "Instância cancelada com sucesso" });
    }

    [HttpPost("series/{serieId:guid}/cancel")]
    public async Task<IActionResult> CancelSeries(Guid serieId)
    {
        var companyId = GetCompanyId();
        await _mediator.Send(new CancelSeriesCommand(companyId, serieId));
        return Ok(new { message = "Série cancelada com sucesso" });
    }

    private Guid GetCompanyId()
    {
        var claim = User.FindFirst("company_id")?.Value;
        return Guid.TryParse(claim, out var companyId) ? companyId : Guid.Empty;
    }
}

public record CreateRecurringAppointmentRequest(
    string Titulo,
    string? Descricao,
    DateTime DataHoraInicio,
    DateTime DataHoraFim,
    Domain.Enums.TipoCompromisso Tipo,
    Guid? ClientId,
    string RecurrenceTipo,
    int RecurrenceIntervalo = 1,
    int[]? RecurrenceDiasDaSemana = null,
    string RecurrenceFimTipo = "date",
    DateTime? RecurrenceDataFim = null,
    int? RecurrenceContagemFim = null);

public record EditSingleInstanceRequest(
    string Titulo,
    string? Descricao,
    DateTime DataHoraInicio,
    DateTime DataHoraFim,
    Domain.Enums.TipoCompromisso Tipo,
    Guid? ClientId);

public record EditSeriesRequest(
    string Titulo,
    string? Descricao,
    DateTime DataHoraInicio,
    DateTime DataHoraFim,
    Domain.Enums.TipoCompromisso Tipo,
    Guid? ClientId,
    string RecurrenceTipo,
    int RecurrenceIntervalo = 1,
    int[]? RecurrenceDiasDaSemana = null,
    string RecurrenceFimTipo = "date",
    DateTime? RecurrenceDataFim = null,
    int? RecurrenceContagemFim = null);
