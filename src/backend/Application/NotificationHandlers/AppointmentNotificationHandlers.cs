using Severina.Application.Interfaces;
using Severina.Domain.Events;

namespace Severina.Application.NotificationHandlers;

public class AppointmentReminderHandler
{
    private readonly INotificationService _notificationService;

    public AppointmentReminderHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task HandleAsync(AppointmentCreatedEvent notification, CancellationToken cancellationToken = default)
    {
        await _notificationService.SendToCompany(
            notification.CompanyId,
            "appointment_created",
            new
            {
                AppointmentId = notification.AppointmentId,
                Titulo = notification.Titulo,
                DataHoraInicio = notification.DataHoraInicio
            });
    }
}

public class AppointmentStatusChangedHandler
{
    private readonly INotificationService _notificationService;

    public AppointmentStatusChangedHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task HandleAsync(AppointmentStatusChangedEvent notification, CancellationToken cancellationToken = default)
    {
        await _notificationService.SendToCompany(
            notification.CompanyId,
            "appointment_status_changed",
            new
            {
                AppointmentId = notification.AppointmentId,
                OldStatus = notification.OldStatus,
                NewStatus = notification.NewStatus
            });
    }
}

public class AppointmentCancelledNotificationHandler
{
    private readonly INotificationService _notificationService;

    public AppointmentCancelledNotificationHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task HandleAsync(AppointmentCancelledEvent notification, CancellationToken cancellationToken = default)
    {
        await _notificationService.SendToCompany(
            notification.CompanyId,
            "appointment_cancelled",
            new
            {
                AppointmentId = notification.AppointmentId,
                Titulo = notification.Titulo,
                OldStatus = notification.OldStatus
            });
    }
}
