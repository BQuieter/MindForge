using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MindForgeServer
{
    [Authorize]
    public class PersonalChatHub : Hub 
    {
    }
}
