using FluentValidation;
using MediatR;
using Severina.Application.DTOs;
using Severina.Application.Interfaces;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;
using Severina.Domain.ValueObjects;

namespace Severina.Application.Commands.Appointments;

public record EditSeriesCommand(
    Guid CompanyId,
    Guid SerieId,
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
    int? RecurrenceContagemFim = null) : IRequest<Unit>;

public class EditSeriesCommandValidator : AbstractValidator<EditSeriesCommand>
{
    public EditSeriesCommandValidator()
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

public class EditSeriesCommandHandler : IRequestHandler<EditSeriesCommand, Unit>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentCacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;

    public EditSeriesCommandHandler(
        IAppointmentRepository appointmentRepository,
        IAppointmentCacheService cacheService,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(EditSeriesCommand request, CancellationToken cancellationToken)
    {
        var appointments = await _appointmentRepository.GetByDateRangeAsync(
            request.CompanyId, DateTime.UtcNow.AddYears(-1), DateTime.UtcNow.AddYears(2));

        var serieAppointments = appointments.Where(a => a.SerieId == request.SerieId).ToList();

        if (!serieAppointments.Any())
            throw new InvalidOperationException("Série não encontrada");

        var rule = RecurrenceRule.Create(
            request.RecurrenceTipo,
            request.RecurrenceIntervalo,
            request.RecurrenceDiasDaSemana,
            request.RecurrenceFimTipo,
            request.RecurrenceDataFim,
            request.RecurrenceContagemFim);

        foreach (var appointment in serieAppointments)
        {
            appointment.UpdateDetails(request.Titulo, request.Descricao, request.Tipo, request.ClientId);
            appointment.SetRecurrenceRule(rule);
        }

        await _cacheService.InvalidateSeriesAsync(request.SerieId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
