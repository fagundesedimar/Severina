namespace Severina.Application.Interfaces;

public interface INotificationService
{
    Task SendToCompany(Guid companyId, string eventName, object data);
    Task SendToUser(Guid userId, string eventName, object data);
}
