using MediatR;
using Severina.Application.DTOs;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;

namespace Severina.Application.Queries.Appointments;

public record ListAppointmentsQuery(
    Guid CompanyId,
    DateTime? From,
    DateTime? To,
    Guid? ClientId,
    StatusCompromisso? Status,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedAppointmentResponse>;

public class ListAppointmentsQueryHandler : IRequestHandler<ListAppointmentsQuery, PagedAppointmentResponse>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public ListAppointmentsQueryHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<PagedAppointmentResponse> Handle(ListAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var from = request.From ?? DateTime.UtcNow.AddDays(-7);
        var to = request.To ?? DateTime.UtcNow.AddDays(7);

        var allAppointments = await _appointmentRepository.GetByDateRangeAsync(request.CompanyId, from, to);

        var filtered = allAppointments.AsEnumerable();

        if (request.ClientId.HasValue)
            filtered = filtered.Where(a => a.ClientId == request.ClientId.Value);

        if (request.Status.HasValue)
            filtered = filtered.Where(a => a.Status == request.Status.Value);

        var totalCount = filtered.Count();
        var items = filtered
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new AppointmentResponse(
                a.Id, a.CompanyId, a.ClientId, a.Titulo, a.Descricao,
                a.DataHoraInicio, a.DataHoraFim, a.Tipo, a.Status,
                a.RecurrenceRuleJson, a.SerieId, a.CreatedAt, a.UpdatedAt))
            .ToList();

        return new PagedAppointmentResponse(items, totalCount, request.Page, request.PageSize);
    }
}
