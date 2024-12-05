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
        public event EventHandler<MessageSentEventArgs> MessageSent;


        public async Task StartAsync()
        {
            connection = new HubConnectionBuilder().WithUrl(App.HttpsStr + "/personalchathub", options =>
            {
                var token = HttpClientSingleton.httpClient!.DefaultRequestHeaders.Authorization!.Parameter;
                options.Headers["Authorization"] = $"Bearer {token}";
            }).WithAutomaticReconnect().Build();

            connection.On<PersonalChatInformation>("ChatCreated", profile => {
                PersonalChatCreated?.Invoke(this, profile);
                Console.WriteLine(profile.ChatId);
            });

            connection.On<MessageInformation, int>("MessageSent", (message, chatId) => {
                MessageSent?.Invoke(this, new MessageSentEventArgs(message,chatId));
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
        event EventHandler<MessageSentEventArgs> MessageSent;
        Task StartAsync();
        Task StopAsync();
    }
}
