using MediatR;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;

namespace Severina.Application.Commands.Appointments;

public record CancelSingleInstanceCommand(Guid CompanyId, Guid InstanceId) : IRequest<Unit>;

public class CancelSingleInstanceCommandHandler : IRequestHandler<CancelSingleInstanceCommand, Unit>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelSingleInstanceCommandHandler(
        IAppointmentRepository appointmentRepository,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(CancelSingleInstanceCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(request.InstanceId)
            ?? throw new InvalidOperationException("Instância não encontrada");

        if (appointment.CompanyId != request.CompanyId)
            throw new InvalidOperationException("Instância não encontrada");

        appointment.Cancel();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
