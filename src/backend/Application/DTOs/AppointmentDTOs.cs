using Severina.Domain.Enums;

namespace Severina.Application.DTOs;

public record AppointmentResponse(
    Guid Id,
    Guid CompanyId,
    Guid? ClientId,
    string Titulo,
    string? Descricao,
    DateTime DataHoraInicio,
    DateTime DataHoraFim,
    TipoCompromisso Tipo,
    StatusCompromisso Status,
    string? RecurrenceRuleJson,
    Guid? SerieId,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record CreateAppointmentRequest(
    string Titulo,
    string? Descricao,
    DateTime DataHoraInicio,
    DateTime DataHoraFim,
    TipoCompromisso Tipo,
    Guid? ClientId);

public record UpdateAppointmentRequest(
    string Titulo,
    string? Descricao,
    DateTime DataHoraInicio,
    DateTime DataHoraFim,
    TipoCompromisso Tipo,
    Guid? ClientId);

public record PagedAppointmentResponse(
    IReadOnlyList<AppointmentResponse> Items,
    int TotalCount,
    int Page,
    int PageSize);
