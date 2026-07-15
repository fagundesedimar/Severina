using System.Collections.Concurrent;
using System.Text.Json;
using Severina.Application.Commands.Users;
using Severina.Application.Interfaces;

namespace Severina.Infrastructure.Services;

public class InMemoryInviteCacheService : IInviteCacheService
{
    private static readonly ConcurrentDictionary<string, string> _invites = new();

    public Task SaveInviteAsync(string code, InviteData invite)
    {
        var json = JsonSerializer.Serialize(invite);
        _invites[code] = json;
        return Task.CompletedTask;
    }

    public Task<InviteData?> GetInviteAsync(string code)
    {
        if (_invites.TryGetValue(code, out var json))
        {
            var invite = JsonSerializer.Deserialize<InviteData>(json);
            if (invite != null && invite.ExpiresAt >= DateTime.UtcNow)
                return Task.FromResult<InviteData?>(invite);

            _invites.TryRemove(code, out _);
        }

        return Task.FromResult<InviteData?>(null);
    }

    public Task DeleteInviteAsync(string code)
    {
        _invites.TryRemove(code, out _);
        return Task.CompletedTask;
    }

    public Task<List<InviteData>> GetPendingInvitesAsync(Guid companyId)
    {
        var result = new List<InviteData>();

        foreach (var kvp in _invites)
        {
            var invite = JsonSerializer.Deserialize<InviteData>(kvp.Value);
            if (invite != null && invite.CompanyId == companyId && invite.ExpiresAt >= DateTime.UtcNow)
            {
                result.Add(invite);
            }
        }

        return Task.FromResult(result);
    }
}
