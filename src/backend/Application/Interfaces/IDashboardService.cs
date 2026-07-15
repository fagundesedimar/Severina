using Severina.Application.DTOs;

namespace Severina.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardResponse> GetDashboardAsync(Guid companyId, CancellationToken cancellationToken = default);
}
