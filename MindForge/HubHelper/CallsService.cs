using MindForgeClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using MindForge;
using System.Net.Http;
using System.Windows.Threading;
using System.Windows;

namespace MindForgeClient
{
    internal class CallsService : ICallsService
    {
        public HubConnection connection;
        public event EventHandler<MemberEventArgs> UserJoined;
        public event EventHandler<MemberEventArgs> UserLeaved;
        public event EventHandler<MemberEventArgs> CallCreated;

        public async Task StartAsync()
        {
            connection = new HubConnectionBuilder().WithUrl(App.HttpsStr + "/callshub", options =>
            {
                var token = HttpClientSingleton.httpClient!.DefaultRequestHeaders.Authorization!.Parameter;
                options.Headers["Authorization"] = $"Bearer {token}";
            }).WithAutomaticReconnect().Build();

            connection.On<ProfileInformation,int>("UserJoin", (profile, chatId) => {
                UserJoined?.Invoke(this, new MemberEventArgs(profile, chatId));
            });

            connection.On<ProfileInformation, int>("UserLeave", (profile, chatId) => {
                UserLeaved?.Invoke(this, new MemberEventArgs(profile, chatId));
            });

            connection.On<ProfileInformation, int>("CallCreate", (profile, chatId) => {
                CallCreated?.Invoke(this, new MemberEventArgs(profile, chatId));
            });


            try
            {
                await connection.StartAsync();
            }
            catch (Exception ex)
            {
                await Task.Delay(1000);
                await connection.StartAsync();
            }
        }
        public HubConnection GetConnection()
        {
            return connection;
        }
        public async void JoinGroup( int chatId)
        {
            await connection.SendAsync("Enter", chatId);
        }
        public async void LeaveGroup(int chatId)
        {
            await connection.SendAsync("Leave",chatId);
        }
        public async Task StopAsync()
        {
            if (connection != null && connection.State == HubConnectionState.Connected)
            {
                await connection.StopAsync();
            }
        }
    }
    public interface ICallsService
    {
        event EventHandler<MemberEventArgs> UserJoined;
        event EventHandler<MemberEventArgs> UserLeaved;
        event EventHandler<MemberEventArgs> CallCreated;
        public void JoinGroup(int chatId);
        public HubConnection GetConnection();
        public void LeaveGroup(int chatId);
        //void FireYouDeletedEvent(object sender, int chatId);
        Task StartAsync();
        Task StopAsync();
    }
}
