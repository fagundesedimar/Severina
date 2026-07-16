namespace Severina.Domain.Events;

public record InvoiceCreatedEvent(Guid InvoiceId, Guid CompanyId, decimal Valor, DateTime DataVencimento) : DomainEventBase;

public record InvoicePaidEvent(Guid InvoiceId, Guid CompanyId, decimal Valor, decimal ValorPago) : DomainEventBase;

public record InvoiceOverdueEvent(Guid InvoiceId, Guid CompanyId, decimal Valor, DateTime DataVencimento) : DomainEventBase;
