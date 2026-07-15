using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Severina.Application.DTOs;
using Severina.Application.Interfaces;

namespace Severina.Infrastructure.Services;

public class DashboardCacheService : IDashboardCacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<DashboardCacheService> _logger;
    private static readonly TimeSpan DefaultTtl = TimeSpan.FromMinutes(5);
    private const string KeyPrefix = "dashboard:";

    public DashboardCacheService(IDistributedCache cache, ILogger<DashboardCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<DashboardResponse?> GetAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        var key = BuildKey(companyId);
        try
        {
            var cached = await _cache.GetStringAsync(key, cancellationToken);
            if (string.IsNullOrEmpty(cached))
                return null;

            return JsonSerializer.Deserialize<DashboardResponse>(cached);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get dashboard cache for company {CompanyId}", companyId);
            return null;
        }
    }

    public async Task SetAsync(Guid companyId, DashboardResponse dashboard, CancellationToken cancellationToken = default)
    {
        var key = BuildKey(companyId);
        try
        {
            var json = JsonSerializer.Serialize(dashboard);
            await _cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = DefaultTtl
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to set dashboard cache for company {CompanyId}", companyId);
        }
    }

    public async Task InvalidateAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        var key = BuildKey(companyId);
        try
        {
            await _cache.RemoveAsync(key, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to invalidate dashboard cache for company {CompanyId}", companyId);
        }
    }

    private static string BuildKey(Guid companyId)
    {
        return $"{KeyPrefix}{companyId}";
    }
}
