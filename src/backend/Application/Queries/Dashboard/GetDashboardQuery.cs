using MediatR;
using Severina.Application.DTOs;
using Severina.Application.Interfaces;

namespace Severina.Application.Queries.Dashboard;

public record GetDashboardQuery(
    Guid CompanyId) : IRequest<DashboardResponse>;

public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, DashboardResponse>
{
    private readonly IDashboardService _dashboardService;
    private readonly IDashboardCacheService _cacheService;

    public GetDashboardQueryHandler(
        IDashboardService dashboardService,
        IDashboardCacheService cacheService)
    {
        _dashboardService = dashboardService;
        _cacheService = cacheService;
    }

    public async Task<DashboardResponse> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var cached = await _cacheService.GetAsync(request.CompanyId, cancellationToken);
        if (cached is not null)
            return cached;

        var dashboard = await _dashboardService.GetDashboardAsync(request.CompanyId, cancellationToken);
        await _cacheService.SetAsync(request.CompanyId, dashboard, cancellationToken);
        return dashboard;
    }
}
