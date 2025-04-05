using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Swapify.API.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class StatusHub : Hub
{
    private static readonly HashSet<string> OnlineUsers = new();

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Context.ConnectionId;
        OnlineUsers.Add(userId);
        await Clients.All.SendAsync("UserStatusChanged", userId, true);
        await Clients.Caller.SendAsync("OnlineUsers", OnlineUsers.ToList());
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Context.ConnectionId;
        OnlineUsers.Remove(userId);
        await Clients.All.SendAsync("UserStatusChanged", userId, false);
        await base.OnDisconnectedAsync(exception);
    }

    public static IReadOnlyCollection<string> GetOnlineUsers() => OnlineUsers.ToList();
}
