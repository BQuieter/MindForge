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
    internal class PersonalChatNotificationService : IPersonalChatNotificationService
    {
        private HubConnection connection;
        public event EventHandler<PersonalChatInformation> PersonalChatCreated;
        public event EventHandler<GroupChatInformation> GroupChatCreated;
        public event EventHandler<MessageSentEventArgs> MessageSent;
        public event EventHandler<MemberAddEventArgs> MemberAdded;
        public event EventHandler<MemberEventArgs> MemberDeleted;
        public event EventHandler<int> YouDeleted;

        public async Task StartAsync()
        {
            connection = new HubConnectionBuilder().WithUrl(App.HttpsStr + "/personalchathub", options =>
            {
                var token = HttpClientSingleton.httpClient!.DefaultRequestHeaders.Authorization!.Parameter;
                options.Headers["Authorization"] = $"Bearer {token}";
            }).WithAutomaticReconnect().Build();

            connection.On<PersonalChatInformation>("PersonalChatCreated", chat => {
                PersonalChatCreated?.Invoke(this, chat);
            });

            connection.On<GroupChatInformation>("GroupChatCreated", chat => {
                GroupChatCreated?.Invoke(this, chat);
            });

            connection.On<MessageInformation, int, bool>("MessageSent", (message, chatId, isGroup) => {
                MessageSent?.Invoke(this, new MessageSentEventArgs(message,chatId, isGroup));
            });

            connection.On<List<ProfileInformation>, int>("AddMember", (List, chatId) => {
                MemberAdded?.Invoke(this,new MemberAddEventArgs(List,chatId));
            });

            connection.On<ProfileInformation, int>("DeleteMember", (person, chatId) => {
                MemberDeleted?.Invoke(this, new MemberEventArgs(person, chatId));
            });

            connection.On<int>("YouDeleted", (chatId) => {
                YouDeleted?.Invoke(this, chatId);
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
        public void FireYouDeletedEvent(object sender, int chatId)
        {
            YouDeleted?.Invoke(this, chatId);
        }
        public async Task StopAsync()
        {
            if (connection != null && connection.State == HubConnectionState.Connected)
            {
                await connection.StopAsync();
            }
        }
    }
    public interface IPersonalChatNotificationService
    {
        event EventHandler<PersonalChatInformation> PersonalChatCreated;
        event EventHandler<GroupChatInformation> GroupChatCreated;
        event EventHandler<MessageSentEventArgs> MessageSent;
        event EventHandler<MemberAddEventArgs> MemberAdded;
        event EventHandler<MemberEventArgs> MemberDeleted;
        event EventHandler<int> YouDeleted;
        void FireYouDeletedEvent(object sender, int chatId);
        Task StartAsync();
        Task StopAsync();
    }
}
