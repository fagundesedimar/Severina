namespace Severina.Application.Interfaces;

public interface IFinancialCacheService
{
    Task<T?> GetDashboardAsync<T>(Guid companyId) where T : class;
    Task SetDashboardAsync<T>(Guid companyId, T data, TimeSpan? expiry = null) where T : class;
    Task InvalidateDashboardAsync(Guid companyId);
}
