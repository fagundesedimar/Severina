using MediatR;
using Severina.Application.Interfaces;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;

namespace Severina.Application.Commands.Appointments;

public record CancelSeriesCommand(Guid CompanyId, Guid SerieId) : IRequest<Unit>;

public class CancelSeriesCommandHandler : IRequestHandler<CancelSeriesCommand, Unit>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentCacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;

    public CancelSeriesCommandHandler(
        IAppointmentRepository appointmentRepository,
        IAppointmentCacheService cacheService,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(CancelSeriesCommand request, CancellationToken cancellationToken)
    {
        var appointments = await _appointmentRepository.GetByDateRangeAsync(
            request.CompanyId, DateTime.UtcNow.AddYears(-1), DateTime.UtcNow.AddYears(2));

        var serieAppointments = appointments.Where(a => a.SerieId == request.SerieId).ToList();

        if (!serieAppointments.Any())
            throw new InvalidOperationException("Série não encontrada");

        foreach (var appointment in serieAppointments)
        {
            if (appointment.Status != StatusCompromisso.Cancelled)
            {
                appointment.Cancel();
            }
        }

        await _cacheService.InvalidateSeriesAsync(request.SerieId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
