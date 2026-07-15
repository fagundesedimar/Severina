using MediatR;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;

namespace Severina.Application.Commands.Appointments;

public record DeleteAppointmentCommand(Guid CompanyId, Guid Id) : IRequest<Unit>;

public class DeleteAppointmentCommandHandler : IRequestHandler<DeleteAppointmentCommand, Unit>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(request.Id)
            ?? throw new InvalidOperationException("Appointment não encontrado");

        if (appointment.CompanyId != request.CompanyId)
            throw new InvalidOperationException("Appointment não encontrado");

        appointment.SoftDelete();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
