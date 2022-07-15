using System.Collections.Concurrent;
using ChatApp.Utilities.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.Hubs
{
    public class BaseHub : Hub
    {
        protected static readonly ConcurrentDictionary<string, List<string>> Connections = new();

        public override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var userId = httpContext?.UserId();
            if (userId == null) return null;

            var canGet = Connections.TryGetValue(userId, out var connections);

            if (!canGet)
            {
                Connections.TryAdd(userId, new List<string>() { Context.ConnectionId });
                return Task.CompletedTask;
            }

            base.OnConnectedAsync();

            connections?.Add(Context.ConnectionId);
            return Task.CompletedTask;
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var httpContext = Context.GetHttpContext();
            var userId = httpContext?.UserId();
            if (userId == null) return Task.CompletedTask;

            var canGet = Connections.TryGetValue(userId, out var connections);
            if (!canGet) return Task.CompletedTask;

            var connectionIdIndex = connections.IndexOf(Context.ConnectionId);
            if (connectionIdIndex > -1)
                connections.RemoveAt(connectionIdIndex);

            if (connections.Any())
                return Task.CompletedTask;

            // start =>> What ???????
            Connections.Remove(Context.ConnectionId, out _);
            // end

            return Task.CompletedTask;
        }

        public IReadOnlyCollection<string> GetOnlineUsers(params string[] userIds)
        {
            var result = new List<string>();
            foreach (var userId in userIds)
            {
                var canGet = Connections.TryGetValue(userId, out var connectionIds);
                if (!canGet) continue;
                result.AddRange(connectionIds);
            }

            return result;
        }
    }
}
