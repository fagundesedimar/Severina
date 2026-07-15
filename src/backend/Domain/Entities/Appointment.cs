using Severina.Domain.Enums;
using Severina.Domain.Events;
using Severina.Domain.ValueObjects;

namespace Severina.Domain.Entities;

public class Appointment : BaseEntity
{
    public Guid CompanyId { get; private set; }
    public Guid? ClientId { get; private set; }
    public string Titulo { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public DateTime DataHoraInicio { get; private set; }
    public DateTime DataHoraFim { get; private set; }
    public TipoCompromisso Tipo { get; private set; }
    public StatusCompromisso Status { get; private set; }
    public string? RecurrenceRuleJson { get; private set; }
    public Guid? SerieId { get; private set; }

    public Company? Company { get; private set; }

    private Appointment() { }

    public Appointment(Guid companyId, string titulo, DateTime dataHoraInicio, DateTime dataHoraFim, TipoCompromisso tipo, Guid? clientId = null, string? descricao = null)
    {
        CompanyId = companyId;
        Titulo = titulo;
        DataHoraInicio = dataHoraInicio;
        DataHoraFim = dataHoraFim;
        Tipo = tipo;
        ClientId = clientId;
        Descricao = descricao;
        Status = StatusCompromisso.Scheduled;

        AddDomainEvent(new AppointmentCreatedEvent(Id, CompanyId, Titulo, DataHoraInicio));
    }

    public void Confirm()
    {
        if (Status == StatusCompromisso.Cancelled)
            throw new InvalidOperationException("Não é possível confirmar appointment cancelado.");

        if (Status == StatusCompromisso.Completed)
            throw new InvalidOperationException("Appointment já foi concluído.");

        var oldStatus = Status;
        Status = StatusCompromisso.Confirmed;
        UpdateTimestamp();
        AddDomainEvent(new AppointmentStatusChangedEvent(Id, CompanyId, oldStatus.ToString(), Status.ToString()));
    }

    public void Cancel()
    {
        if (Status == StatusCompromisso.Completed)
            throw new InvalidOperationException("Appointment já foi concluído.");

        if (Status == StatusCompromisso.Cancelled)
            throw new InvalidOperationException("Appointment já está cancelado.");

        var oldStatus = Status;
        Status = StatusCompromisso.Cancelled;
        UpdateTimestamp();
        AddDomainEvent(new AppointmentCancelledEvent(Id, CompanyId, Titulo, oldStatus.ToString()));
    }

    public void Complete()
    {
        if (Status == StatusCompromisso.Cancelled)
            throw new InvalidOperationException("Appointment cancelado não pode ser concluído.");

        if (Status == StatusCompromisso.Completed)
            throw new InvalidOperationException("Appointment já foi concluído.");

        if (DataHoraInicio > DateTime.UtcNow)
            throw new InvalidOperationException("Appointment ainda não começou.");

        var oldStatus = Status;
        Status = StatusCompromisso.Completed;
        UpdateTimestamp();
        AddDomainEvent(new AppointmentStatusChangedEvent(Id, CompanyId, oldStatus.ToString(), Status.ToString()));
    }

    public void Reschedule(DateTime novaDataHoraInicio, DateTime novaDataHoraFim)
    {
        if (Status == StatusCompromisso.Completed)
            throw new InvalidOperationException("Appointment concluído não pode ser reagendado.");

        if (Status == StatusCompromisso.Cancelled)
            throw new InvalidOperationException("Appointment cancelado não pode ser reagendado.");

        if (novaDataHoraFim <= novaDataHoraInicio)
            throw new ArgumentException("Horário de fim deve ser após horário de início.");

        DataHoraInicio = novaDataHoraInicio;
        DataHoraFim = novaDataHoraFim;
        UpdateTimestamp();
    }

    public void UpdateDetails(string titulo, string? descricao, TipoCompromisso tipo, Guid? clientId)
    {
        Titulo = titulo;
        Descricao = descricao;
        Tipo = tipo;
        ClientId = clientId;
        UpdateTimestamp();
    }

    public void SetRecurrenceRule(RecurrenceRule? rule)
    {
        RecurrenceRuleJson = rule?.ToJson();
        UpdateTimestamp();
    }

    public void SetSerieId(Guid serieId)
    {
        SerieId = serieId;
        UpdateTimestamp();
    }

    public bool HasRecurrence => !string.IsNullOrEmpty(RecurrenceRuleJson);

    public RecurrenceRule? GetRecurrenceRule()
    {
        if (string.IsNullOrEmpty(RecurrenceRuleJson))
            return null;

        return RecurrenceRule.FromJson(RecurrenceRuleJson);
    }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
