using MediatR;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;

namespace Severina.Application.Commands.Appointments;

public record CompleteAppointmentCommand(Guid CompanyId, Guid Id) : IRequest<Unit>;

public class CompleteAppointmentCommandHandler : IRequestHandler<CompleteAppointmentCommand, Unit>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(CompleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(request.Id)
            ?? throw new InvalidOperationException("Appointment não encontrado");

        if (appointment.CompanyId != request.CompanyId)
            throw new InvalidOperationException("Appointment não encontrado");

        appointment.Complete();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
