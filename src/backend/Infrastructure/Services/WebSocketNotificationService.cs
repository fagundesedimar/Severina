using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text.Json;
using Severina.Application.Interfaces;

namespace Severina.Infrastructure.Services;

public class WebSocketNotificationService : INotificationService
{
    private static readonly ConcurrentDictionary<Guid, ConcurrentBag<WebSocket>> _companyConnections = new();
    private static readonly ConcurrentDictionary<Guid, ConcurrentBag<WebSocket>> _userConnections = new();

    public static void RegisterConnection(Guid companyId, Guid? userId, WebSocket socket)
    {
        _companyConnections.GetOrAdd(companyId, _ => new ConcurrentBag<WebSocket>()).Add(socket);

        if (userId.HasValue)
            _userConnections.GetOrAdd(userId.Value, _ => new ConcurrentBag<WebSocket>()).Add(socket);
    }

    public static void RemoveConnection(Guid companyId, Guid? userId, WebSocket socket)
    {
        if (_companyConnections.TryGetValue(companyId, out var connections))
        {
            var newBag = new ConcurrentBag<WebSocket>(connections.Where(c => c != socket));
            _companyConnections[companyId] = newBag;
        }

        if (userId.HasValue && _userConnections.TryGetValue(userId.Value, out var userConnections))
        {
            var newUserBag = new ConcurrentBag<WebSocket>(userConnections.Where(c => c != socket));
            _userConnections[userId.Value] = newUserBag;
        }
    }

    public async Task SendToCompany(Guid companyId, string eventName, object data)
    {
        if (!_companyConnections.TryGetValue(companyId, out var connections))
            return;

        var message = JsonSerializer.Serialize(new { Event = eventName, Data = data });
        var bytes = System.Text.Encoding.UTF8.GetBytes(message);

        foreach (var socket in connections.ToList())
        {
            if (socket.State == WebSocketState.Open)
            {
                try
                {
                    await socket.SendAsync(
                        new ArraySegment<byte>(bytes),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None);
                }
                catch
                {
                    RemoveConnection(companyId, null, socket);
                }
            }
        }
    }

    public async Task SendToUser(Guid userId, string eventName, object data)
    {
        if (!_userConnections.TryGetValue(userId, out var connections))
            return;

        var message = JsonSerializer.Serialize(new { Event = eventName, Data = data });
        var bytes = System.Text.Encoding.UTF8.GetBytes(message);

        foreach (var socket in connections.ToList())
        {
            if (socket.State == WebSocketState.Open)
            {
                try
                {
                    await socket.SendAsync(
                        new ArraySegment<byte>(bytes),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None);
                }
                catch
                {
                    RemoveConnection(Guid.Empty, userId, socket);
                }
            }
        }
    }
}
