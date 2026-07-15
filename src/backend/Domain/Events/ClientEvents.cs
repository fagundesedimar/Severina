namespace Severina.Domain.Events;

public record ClientCreatedEvent(Guid ClientId, Guid CompanyId, string Nome) : DomainEventBase;
public record ClientUpdatedEvent(Guid ClientId, Guid CompanyId) : DomainEventBase;
public record ClientTagAddedEvent(Guid ClientId, Guid CompanyId, string TagName) : DomainEventBase;
