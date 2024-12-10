using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualBasic;
using MindForgeDbClasses;
using NAudio.Wave;

namespace MindForgeServer
{
    [Authorize]
    public class CallsHub : Hub
    {
        public async Task Enter(int chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
        }
        public async Task Leave(int chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
        }

        public async Task Send(byte[] audio, int chatId, WaveFormat waveFormat)
        {
            Clients.OthersInGroup(chatId.ToString()).SendAsync("GetAudio", audio, waveFormat);
        }
    }
}
