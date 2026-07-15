using FluentValidation;
using MediatR;
using Severina.Application.DTOs;
using Severina.Domain.Entities;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;
using Severina.Domain.ValueObjects;

namespace Severina.Application.Commands.Appointments;

public record CreateRecurringAppointmentCommand(
    Guid CompanyId,
    string Titulo,
    string? Descricao,
    DateTime DataHoraInicio,
    DateTime DataHoraFim,
    TipoCompromisso Tipo,
    Guid? ClientId,
    string RecurrenceTipo,
    int RecurrenceIntervalo = 1,
    int[]? RecurrenceDiasDaSemana = null,
    string RecurrenceFimTipo = "date",
    DateTime? RecurrenceDataFim = null,
    int? RecurrenceContagemFim = null) : IRequest<AppointmentResponse>;

public class CreateRecurringAppointmentCommandValidator : AbstractValidator<CreateRecurringAppointmentCommand>
{
    public CreateRecurringAppointmentCommandValidator()
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

        RuleFor(x => x.RecurrenceTipo)
            .NotEmpty().WithMessage("Tipo de recorrência é obrigatório")
            .Must(t => new[] { "daily", "weekly", "monthly", "custom" }.Contains(t.ToLowerInvariant()))
            .WithMessage("Tipo de recorrência inválido");

        RuleFor(x => x.RecurrenceFimTipo)
            .Must(t => t == "date" || t == "count")
            .WithMessage("FimTipo deve ser 'date' ou 'count'");

        When(x => x.RecurrenceFimTipo == "date", () =>
        {
            RuleFor(x => x.RecurrenceDataFim)
                .NotEmpty().WithMessage("Data de fim é obrigatória para recorrência por data");
        });

        When(x => x.RecurrenceFimTipo == "count", () =>
        {
            RuleFor(x => x.RecurrenceContagemFim)
                .NotNull().WithMessage("Contagem de fim é obrigatória para recorrência por contagem")
                .GreaterThan(0).WithMessage("Contagem de fim deve ser maior que zero");
        });
    }
}

public class CreateRecurringAppointmentCommandHandler : IRequestHandler<CreateRecurringAppointmentCommand, AppointmentResponse>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRecurringAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AppointmentResponse> Handle(CreateRecurringAppointmentCommand request, CancellationToken cancellationToken)
    {
        var rule = RecurrenceRule.Create(
            request.RecurrenceTipo,
            request.RecurrenceIntervalo,
            request.RecurrenceDiasDaSemana,
            request.RecurrenceFimTipo,
            request.RecurrenceDataFim,
            request.RecurrenceContagemFim);

        var serieId = Guid.NewGuid();

        var appointment = new Appointment(
            request.CompanyId,
            request.Titulo,
            request.DataHoraInicio,
            request.DataHoraFim,
            request.Tipo,
            request.ClientId,
            request.Descricao);

        appointment.SetRecurrenceRule(rule);
        appointment.SetSerieId(serieId);

        await _appointmentRepository.AddAsync(appointment);
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
