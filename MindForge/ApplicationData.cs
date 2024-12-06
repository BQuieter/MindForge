using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.AspNetCore.SignalR.Client;
using MindForge;
using MindForgeClasses;

namespace MindForgeClient
{
    internal class ApplicationData
    {
        private HttpClient httpClient;
        public ProfileInformation UserProfile { get; set; }
        public ObservableCollection<ProfessionInformation> UserProfessions { get; set; }
        public ObservableCollection<ProfileInformation> UsersFriends { get; set; }
        public ObservableCollection<ProfileInformation> UsersIncomingRequests { get; set; }
        public ObservableCollection<ProfileInformation> UsersOutgoingRequests { get; set; }
        public List<ProfessionInformation> AllProfessions { get; set; }
        public ObservableCollection<PersonalChatInformation> PersonalChatsInformation { get; set; }
        public ObservableCollection<GroupChatInformation> GroupChatInformation { get; set; }
        public Dictionary<int, ObservableCollection<MessageGroup>> PersonalChats { get; set; } = new();
        public Dictionary<int, ObservableCollection<MessageGroup>> GroupChats { get; set; } = new();
        private readonly TaskCompletionSource<bool> loadedTaskSource = new();
        public Task<bool> LoadedTask => loadedTaskSource.Task; 
    
        public ApplicationData() 
        {
            httpClient = HttpClientSingleton.httpClient!;
            LoadInformation();
        }
        private async Task LoadInformation()
        {
            try
            {
                await GetUserProfile();
                await GetAllProfessions();
                await GetUserProfessions();
                await GetUserFriends();
                await GetUserIncomingRequests();
                await GetUserOutgoingRequests();
                await GetPersonalChats();
                loadedTaskSource.SetResult(true);
            }
            catch (Exception ex)
            {
                loadedTaskSource.SetException(ex); 
            }
        }

        private async Task GetUserProfile()
        {
            var response = await httpClient.GetAsync(App.HttpsStr + "/profile");
            if (!response.IsSuccessStatusCode)
                return;
            UserProfile = await response.Content.ReadFromJsonAsync<ProfileInformation>();
        }
        private async Task GetAllProfessions()
        {
            var response = await httpClient.GetAsync(App.HttpsStr + "/professions");
            if (!response.IsSuccessStatusCode)
                return;
            AllProfessions = await response.Content.ReadFromJsonAsync<List<ProfessionInformation>>();
        }

        private async Task GetUserProfessions()
        {
            var response = await httpClient.GetAsync(App.HttpsStr + $"/professions/{UserProfile.Login}");
            if (!response.IsSuccessStatusCode)
                return;
            UserProfessions = new ObservableCollection<ProfessionInformation>(await response.Content.ReadFromJsonAsync<List<ProfessionInformation>>());
        }
        private async Task GetUserFriends()
        {
            var response = await httpClient.GetAsync(App.HttpsStr + "/friends/all");
            if (!response.IsSuccessStatusCode)
                return;
            UsersFriends = new ObservableCollection<ProfileInformation>(await response.Content.ReadFromJsonAsync<List<ProfileInformation>>());
        }
        private async Task GetUserIncomingRequests()
        {
            var response = await httpClient.GetAsync(App.HttpsStr + $"/requests/incoming");
            if (!response.IsSuccessStatusCode)
                return;
            UsersIncomingRequests = new ObservableCollection<ProfileInformation>(await response.Content.ReadFromJsonAsync<List<ProfileInformation>>());
        }
        private async Task GetUserOutgoingRequests()
        {
            var response = await httpClient.GetAsync(App.HttpsStr + $"/requests/outgoing");
            if (!response.IsSuccessStatusCode)
                return;
            UsersOutgoingRequests = new ObservableCollection<ProfileInformation>(await response.Content.ReadFromJsonAsync<List<ProfileInformation>>());
        }
        private async Task GetPersonalChats()
        {
            var response = await httpClient.GetAsync(App.HttpsStr + "/personalchats");
            if (!response.IsSuccessStatusCode)
                return;
            PersonalChatsInformation = new ObservableCollection<PersonalChatInformation>(await response.Content.ReadFromJsonAsync<List<PersonalChatInformation>>());
            foreach(var a in PersonalChatsInformation)
            {
                PersonalChats.Add(a.ChatId,new());
            }
        }
        private async Task GetGroupChats()
        {
            var response = await httpClient.GetAsync(App.HttpsStr + "/groupchats");
            if (!response.IsSuccessStatusCode)
                return;
            GroupChatInformation = new ObservableCollection<GroupChatInformation>(await response.Content.ReadFromJsonAsync<List<GroupChatInformation>>());
            foreach (var a in GroupChatInformation)
            {
                GroupChats.Add(a.ChatId, new());
            }
        }
    }
}
