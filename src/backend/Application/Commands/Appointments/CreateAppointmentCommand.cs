using FluentValidation;
using MediatR;
using Severina.Application.DTOs;
using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;

namespace Severina.Application.Commands.Appointments;

public record CreateAppointmentCommand(
    Guid CompanyId,
    string Titulo,
    string? Descricao,
    DateTime DataHoraInicio,
    DateTime DataHoraFim,
    TipoCompromisso Tipo,
    Guid? ClientId) : IRequest<AppointmentResponse>;

public class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentCommandValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("Título é obrigatório")
            .MaximumLength(200).WithMessage("Título deve ter no máximo 200 caracteres");

        RuleFor(x => x.DataHoraInicio)
            .NotEmpty().WithMessage("Data/hora de início é obrigatória")
            .Must(d => d > DateTime.UtcNow).WithMessage("Não é possível agendar no passado");

        RuleFor(x => x.DataHoraFim)
            .NotEmpty().WithMessage("Data/hora de fim é obrigatória")
            .GreaterThan(x => x.DataHoraInicio).WithMessage("Horário de fim deve ser após horário de início");
    }
}

public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, AppointmentResponse>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AppointmentResponse> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var conflicts = await _appointmentRepository.GetConflictingAsync(
            request.CompanyId, request.DataHoraInicio, request.DataHoraFim);

        if (conflicts.Any())
            throw new InvalidOperationException("Horário conflita com appointment existente");

        var appointment = new Appointment(
            request.CompanyId,
            request.Titulo,
            request.DataHoraInicio,
            request.DataHoraFim,
            request.Tipo,
            request.ClientId,
            request.Descricao);

        await _appointmentRepository.AddAsync(appointment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(appointment);
    }

    private static AppointmentResponse MapToResponse(Appointment a)
    {
        return new AppointmentResponse(
            a.Id, a.CompanyId, a.ClientId, a.Titulo, a.Descricao,
            a.DataHoraInicio, a.DataHoraFim, a.Tipo, a.Status,
            a.RecurrenceRuleJson, a.SerieId, a.CreatedAt, a.UpdatedAt);
    }
}
