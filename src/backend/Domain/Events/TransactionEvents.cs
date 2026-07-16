using Severina.Domain.Enums;

namespace Severina.Domain.Events;

public record TransactionCreatedEvent(Guid TransactionId, Guid CompanyId, TransactionType Tipo, decimal Valor, DateTime Data) : DomainEventBase;

public record TransactionApprovedEvent(Guid TransactionId, Guid CompanyId, string OldStatus, string NewStatus) : DomainEventBase;

public record TransactionRejectedEvent(Guid TransactionId, Guid CompanyId, string OldStatus, string NewStatus) : DomainEventBase;
