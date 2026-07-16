using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Severina.Application.Interfaces;

namespace Severina.Infrastructure.Services;

public class FinancialCacheService : IFinancialCacheService
{
    private readonly IDistributedCache _cache;
    private static readonly TimeSpan DefaultTTL = TimeSpan.FromMinutes(5);

    public FinancialCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetDashboardAsync<T>(Guid companyId) where T : class
    {
        var key = $"financial:dashboard:{companyId}";
        var json = await _cache.GetStringAsync(key);
        if (string.IsNullOrEmpty(json))
            return null;

        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetDashboardAsync<T>(Guid companyId, T data, TimeSpan? expiry = null) where T : class
    {
        var key = $"financial:dashboard:{companyId}";
        var json = JsonSerializer.Serialize(data);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry ?? DefaultTTL
        };
        await _cache.SetStringAsync(key, json, options);
    }

    public async Task InvalidateDashboardAsync(Guid companyId)
    {
        var key = $"financial:dashboard:{companyId}";
        await _cache.RemoveAsync(key);
    }
}
