using FluentValidation;
using MediatR;
using Severina.Application.DTOs;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;

namespace Severina.Application.Commands.Appointments;

public record UpdateAppointmentCommand(
    Guid CompanyId,
    Guid Id,
    string Titulo,
    string? Descricao,
    DateTime DataHoraInicio,
    DateTime DataHoraFim,
    TipoCompromisso Tipo,
    Guid? ClientId) : IRequest<AppointmentResponse>;

public class UpdateAppointmentCommandValidator : AbstractValidator<UpdateAppointmentCommand>
{
    public UpdateAppointmentCommandValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("Título é obrigatório")
            .MaximumLength(200).WithMessage("Título deve ter no máximo 200 caracteres");

        RuleFor(x => x.DataHoraInicio)
            .NotEmpty().WithMessage("Data/hora de início é obrigatória");

        RuleFor(x => x.DataHoraFim)
            .NotEmpty().WithMessage("Data/hora de fim é obrigatória")
            .GreaterThan(x => x.DataHoraInicio).WithMessage("Horário de fim deve ser após horário de início");
    }
}

public class UpdateAppointmentCommandHandler : IRequestHandler<UpdateAppointmentCommand, AppointmentResponse>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AppointmentResponse> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(request.Id)
            ?? throw new InvalidOperationException("Appointment não encontrado");

        if (appointment.CompanyId != request.CompanyId)
            throw new InvalidOperationException("Appointment não encontrado");

        if (appointment.DataHoraInicio < DateTime.UtcNow)
            throw new InvalidOperationException("Não é possível alterar appointment passado");

        var conflicts = await _appointmentRepository.GetConflictingAsync(
            request.CompanyId, request.DataHoraInicio, request.DataHoraFim, request.Id);

        if (conflicts.Any())
            throw new InvalidOperationException("Horário conflita com appointment existente");

        appointment.UpdateDetails(request.Titulo, request.Descricao, request.Tipo, request.ClientId);
        appointment.Reschedule(request.DataHoraInicio, request.DataHoraFim);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AppointmentResponse(
            appointment.Id, appointment.CompanyId, appointment.ClientId,
            appointment.Titulo, appointment.Descricao,
            appointment.DataHoraInicio, appointment.DataHoraFim,
            appointment.Tipo, appointment.Status,
            appointment.RecurrenceRuleJson, appointment.SerieId,
            appointment.CreatedAt, appointment.UpdatedAt);
    }
}
