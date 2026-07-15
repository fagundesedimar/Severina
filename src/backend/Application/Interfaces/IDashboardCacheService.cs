using Severina.Application.DTOs;

namespace Severina.Application.Interfaces;

public interface IDashboardCacheService
{
    Task<DashboardResponse?> GetAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task SetAsync(Guid companyId, DashboardResponse dashboard, CancellationToken cancellationToken = default);
    Task InvalidateAsync(Guid companyId, CancellationToken cancellationToken = default);
}
