using FluentValidation;
using MediatR;
using Severina.Application.DTOs;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;

namespace Severina.Application.Commands.Appointments;

public record EditSingleInstanceCommand(
    Guid CompanyId,
    Guid InstanceId,
    string Titulo,
    string? Descricao,
    DateTime DataHoraInicio,
    DateTime DataHoraFim,
    TipoCompromisso Tipo,
    Guid? ClientId) : IRequest<AppointmentResponse>;

public class EditSingleInstanceCommandValidator : AbstractValidator<EditSingleInstanceCommand>
{
    public EditSingleInstanceCommandValidator()
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

public class EditSingleInstanceCommandHandler : IRequestHandler<EditSingleInstanceCommand, AppointmentResponse>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EditSingleInstanceCommandHandler(
        IAppointmentRepository appointmentRepository,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AppointmentResponse> Handle(EditSingleInstanceCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(request.InstanceId)
            ?? throw new InvalidOperationException("Instância não encontrada");

        if (appointment.CompanyId != request.CompanyId)
            throw new InvalidOperationException("Instância não encontrada");

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
