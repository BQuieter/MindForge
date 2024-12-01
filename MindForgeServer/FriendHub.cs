using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace MindForgeServer
{
    [Authorize]
    public class FriendHub : Hub
    {

        [Authorize]
        public async Task DeleteFriend(string message, string to)
        {
            if (Context.UserIdentifier is string userName)
            {
                await Clients.Users(to, userName).SendAsync("Receive", message, userName);
            }
        }
        [Authorize]
        public async Task ApplyRequest(string message, string to)
        {
            if (Context.UserIdentifier is string userName)
            {
                await Clients.Users(to, userName).SendAsync("Receive", message, userName);
            }
        }
        [Authorize]
        public async Task DeleteRequest(string message, string to)
        {
            if (Context.UserIdentifier is string userName)
            {
                await Clients.Users(to, userName).SendAsync("Receive", message, userName);
            }
        }
        [Authorize]
        public async Task RejectRequest(string message, string to)
        {
            if (Context.UserIdentifier is string userName)
            {
                await Clients.Users(to, userName).SendAsync("Receive", message, userName);
            }
        }

    }

    public class CustomUserIdProvider : IUserIdProvider
    {
        public virtual string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.Name)?.Value;
        }
    }
}
