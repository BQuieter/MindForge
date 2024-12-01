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
    internal class FriendNotificationService: IFriendNotificationService
    {
        private HubConnection friendConnection;
        public event EventHandler<ProfileInformation> FriendRequestReceived;
        public event EventHandler<ProfileInformation> FriendRequestRejected;
        public event EventHandler<ProfileInformation> FriendRequestDeleted;
        public event EventHandler<ProfileInformation> FriendDeleted;
        public event EventHandler<ProfileInformation> FriendAdded;

        public async Task StartAsync()
        {
            friendConnection = new HubConnectionBuilder().WithUrl(App.HttpsStr + "/friendhub", options =>
            {
                var token = HttpClientSingleton.httpClient!.DefaultRequestHeaders.Authorization!.Parameter;
                options.Headers["Authorization"] = $"Bearer {token}";
            }).WithAutomaticReconnect().Build();

            friendConnection.On<ProfileInformation>("FriendRequestReceived", profile => {
                FriendRequestReceived?.Invoke(this, profile);
            });

            friendConnection.On<ProfileInformation>("FriendRequestRejected", profile => {
                FriendRequestRejected?.Invoke(this, profile);
            });

            friendConnection.On<ProfileInformation>("FriendDeleted", profile => {
                MessageBox.Show(profile.Login);
                FriendDeleted?.Invoke(this, profile);
            });

            friendConnection.On<ProfileInformation>("FriendAdded", profile => {
                FriendAdded?.Invoke(this, profile);
            });

            friendConnection.On<ProfileInformation>("FriendRequestDeleted", profile => {
                FriendRequestDeleted?.Invoke(this, profile);
            });

            try
            {
                await friendConnection.StartAsync();
            }
            catch (Exception ex) 
            {
                await Task.Delay(1000);
                await friendConnection.StartAsync();
            }
        }
        public async Task StopAsync()
        {
            if (friendConnection != null && friendConnection.State == HubConnectionState.Connected)
            {
                await friendConnection.StopAsync();
            }
        }

    }
    public interface IFriendNotificationService
    {
        event EventHandler<ProfileInformation> FriendRequestReceived;
        event EventHandler<ProfileInformation> FriendRequestRejected;
        event EventHandler<ProfileInformation> FriendRequestDeleted;
        event EventHandler<ProfileInformation> FriendDeleted;
        event EventHandler<ProfileInformation> FriendAdded;
        Task StartAsync();
        Task StopAsync();
    }
}
