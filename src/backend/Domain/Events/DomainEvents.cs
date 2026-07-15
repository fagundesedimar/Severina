namespace Severina.Domain.Events;

public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
}

public abstract record DomainEventBase : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record CompanyCreatedEvent(Guid CompanyId, string Nome, string CnpjCpf) : DomainEventBase;

public record CompanyDeactivatedEvent(Guid CompanyId, string Nome) : DomainEventBase;

public record UserInvitedEvent(Guid CompanyId, string Email, string Papel) : DomainEventBase;

public record UserActivatedEvent(Guid UserId, Guid CompanyId) : DomainEventBase;

public record UserRoleChangedEvent(Guid UserId, Guid CompanyId, string NovoPapel) : DomainEventBase;
