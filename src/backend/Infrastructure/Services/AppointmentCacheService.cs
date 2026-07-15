using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Severina.Application.Interfaces;

namespace Severina.Infrastructure.Services;

public class AppointmentCacheService : IAppointmentCacheService
{
    private readonly IDistributedCache _cache;
    private static readonly TimeSpan DefaultTtl = TimeSpan.FromMinutes(5);

    public AppointmentCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<List<DateTime>> GetInstancesAsync(Guid serieId, DateTime from, DateTime to)
    {
        var key = BuildKey(serieId, from, to);
        var cached = await _cache.GetStringAsync(key);

        if (string.IsNullOrEmpty(cached))
            return new List<DateTime>();

        return JsonSerializer.Deserialize<List<DateTime>>(cached) ?? new List<DateTime>();
    }

    public async Task SetInstancesAsync(Guid serieId, DateTime from, DateTime to, List<DateTime> instances)
    {
        var key = BuildKey(serieId, from, to);
        var json = JsonSerializer.Serialize(instances);

        await _cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = DefaultTtl
        });
    }

    public async Task InvalidateSeriesAsync(Guid serieId)
    {
        var prefix = $"appointment_instances_{serieId}_";
        var redis = _cache as StackExchange.Redis.IDatabase;
        if (redis != null)
        {
            var server = redis.Multiplexer.GetServer(redis.Multiplexer.GetEndPoints().First());
            var keys = server.Keys(pattern: $"{prefix}*");
            foreach (var key in keys)
            {
                await _cache.RemoveAsync(key.ToString());
            }
        }
    }

    private static string BuildKey(Guid serieId, DateTime from, DateTime to)
    {
        return $"appointment_instances_{serieId}_{from:yyyyMMdd}_{to:yyyyMMdd}";
    }
}
