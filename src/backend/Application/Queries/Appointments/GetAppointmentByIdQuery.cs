using MediatR;
using Severina.Application.DTOs;
using Severina.Domain.Interfaces;

namespace Severina.Application.Queries.Appointments;

public record GetAppointmentByIdQuery(Guid CompanyId, Guid Id) : IRequest<AppointmentResponse?>;

public class GetAppointmentByIdQueryHandler : IRequestHandler<GetAppointmentByIdQuery, AppointmentResponse?>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public GetAppointmentByIdQueryHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<AppointmentResponse?> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(request.Id);

        if (appointment is null || appointment.CompanyId != request.CompanyId)
            return null;

        return new AppointmentResponse(
            appointment.Id, appointment.CompanyId, appointment.ClientId,
            appointment.Titulo, appointment.Descricao,
            appointment.DataHoraInicio, appointment.DataHoraFim,
            appointment.Tipo, appointment.Status,
            appointment.RecurrenceRuleJson, appointment.SerieId,
            appointment.CreatedAt, appointment.UpdatedAt);
    }
}
