namespace Severina.Application.Interfaces;

public interface IAppointmentCacheService
{
    Task<List<DateTime>> GetInstancesAsync(Guid serieId, DateTime from, DateTime to);
    Task SetInstancesAsync(Guid serieId, DateTime from, DateTime to, List<DateTime> instances);
    Task InvalidateSeriesAsync(Guid serieId);
}
