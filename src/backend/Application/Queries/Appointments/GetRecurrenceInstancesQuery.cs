using MediatR;
using Severina.Application.DTOs;
using Severina.Application.Interfaces;
using Severina.Domain.Entities;
using Severina.Domain.Interfaces;
using Severina.Domain.ValueObjects;

namespace Severina.Application.Queries.Appointments;

public record GetRecurrenceInstancesQuery(
    Guid CompanyId,
    Guid SerieId,
    DateTime From,
    DateTime To) : IRequest<List<AppointmentResponse>>;

public class GetRecurrenceInstancesQueryHandler : IRequestHandler<GetRecurrenceInstancesQuery, List<AppointmentResponse>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentCacheService _cacheService;

    public GetRecurrenceInstancesQueryHandler(
        IAppointmentRepository appointmentRepository,
        IAppointmentCacheService cacheService)
    {
        _appointmentRepository = appointmentRepository;
        _cacheService = cacheService;
    }

    public async Task<List<AppointmentResponse>> Handle(GetRecurrenceInstancesQuery request, CancellationToken cancellationToken)
    {
        var cachedInstances = await _cacheService.GetInstancesAsync(request.SerieId, request.From, request.To);

        if (cachedInstances.Any())
        {
            return cachedInstances.Select(i => new AppointmentResponse(
                request.SerieId, request.CompanyId, null,
                "Recorrência", null,
                i, i.AddHours(1),
                Domain.Enums.TipoCompromisso.Outro,
                Domain.Enums.StatusCompromisso.Scheduled,
                null, request.SerieId,
                DateTime.UtcNow, DateTime.UtcNow)).ToList();
        }

        var existingAppointments = await _appointmentRepository.GetByDateRangeAsync(request.CompanyId, request.From, request.To);
        var serieAppointments = existingAppointments.Where(a => a.SerieId == request.SerieId).ToList();

        if (!serieAppointments.Any())
            return new List<AppointmentResponse>();

        var baseAppointment = serieAppointments.First();
        var rule = baseAppointment.GetRecurrenceRule();

        if (rule is null)
            return serieAppointments.Select(a => new AppointmentResponse(
                a.Id, a.CompanyId, a.ClientId, a.Titulo, a.Descricao,
                a.DataHoraInicio, a.DataHoraFim, a.Tipo, a.Status,
                a.RecurrenceRuleJson, a.SerieId, a.CreatedAt, a.UpdatedAt)).ToList();

        var instanceDates = RecurrenceGenerator.GenerateInstances(
            baseAppointment.DataHoraInicio, baseAppointment.DataHoraFim,
            rule, request.From, request.To);

        await _cacheService.SetInstancesAsync(request.SerieId, request.From, request.To, instanceDates);

        var duracao = baseAppointment.DataHoraFim - baseAppointment.DataHoraInicio;

        return instanceDates.Select(i => new AppointmentResponse(
            request.SerieId, request.CompanyId, baseAppointment.ClientId,
            baseAppointment.Titulo, baseAppointment.Descricao,
            i, i + duracao, baseAppointment.Tipo,
            Domain.Enums.StatusCompromisso.Scheduled,
            baseAppointment.RecurrenceRuleJson, request.SerieId,
            baseAppointment.CreatedAt, DateTime.UtcNow)).ToList();
    }
}
