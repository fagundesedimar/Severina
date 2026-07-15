using Severina.Domain.Enums;

namespace Severina.Domain.Events;

public record AppointmentCreatedEvent(Guid AppointmentId, Guid CompanyId, string Titulo, DateTime DataHoraInicio) : DomainEventBase;

public record AppointmentStatusChangedEvent(Guid AppointmentId, Guid CompanyId, string OldStatus, string NewStatus) : DomainEventBase;

public record AppointmentCancelledEvent(Guid AppointmentId, Guid CompanyId, string Titulo, string OldStatus) : DomainEventBase;
