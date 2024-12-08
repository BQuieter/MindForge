using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using MindForge;
using MindForgeClasses;
using MindForgeClient.Pages;
using MindForgeClient.Pages.Chats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Shell;

namespace MindForgeClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal ApplicationData applicationData;
        private static HttpClient httpClient;
        internal readonly IFriendNotificationService friendNotificationService;
        internal readonly IPersonalChatNotificationService personalChatNotificationService;
        public MainWindow(IFriendNotificationService friendNotificationService, IPersonalChatNotificationService personalChatNotificationService)
        {
            this.friendNotificationService = friendNotificationService;
            this.personalChatNotificationService = personalChatNotificationService;
            InitializeComponent();
            this.BorderThickness = SystemParametersFix.WindowResizeBorderThickness;
            httpClient = HttpClientSingleton.httpClient!;
            applicationData = new();
            friendNotificationService.FriendRequestReceived += FriendRequestReceive!;
            friendNotificationService.FriendRequestRejected += FriendRequestReject!;
            friendNotificationService.FriendDeleted += FriendDelete!;
            friendNotificationService.FriendAdded += FriendAdd!;
            friendNotificationService.FriendRequestDeleted += FriendRequestDeleted!;
            personalChatNotificationService.PersonalChatCreated += PersonalChatCreate!;
            personalChatNotificationService.GroupChatCreated += GroupChatCreate!;
            personalChatNotificationService.MessageSent += MessageSent!;

            friendNotificationService.StartAsync();
            personalChatNotificationService.StartAsync();
        }


        //Методы обработки ивента прихода данных друзей
        private void FriendRequestReceive(object sender, ProfileInformation profile)
        {
            Dispatcher.Invoke(() =>
                applicationData.UsersIncomingRequests.Add(profile));
        }
        private void FriendRequestReject(object sender, ProfileInformation profile)
        {
            Dispatcher.Invoke(() =>
                applicationData.UsersOutgoingRequests.Remove(applicationData.UsersOutgoingRequests.FirstOrDefault(u => u.Login == profile.Login)));
        }
        private void FriendRequestDeleted(object sender, ProfileInformation profile)
        {
            Dispatcher.Invoke(() =>
                applicationData.UsersIncomingRequests.Remove(applicationData.UsersIncomingRequests.FirstOrDefault(u => u.Login == profile.Login)));
        }
        private void FriendDelete(object sender, ProfileInformation profile)
        {
            Dispatcher.Invoke(() => 
                applicationData.UsersFriends.Remove(applicationData.UsersFriends.FirstOrDefault(u => u.Login == profile.Login)));

        }
        private void FriendAdd(object sender, ProfileInformation profile)
        {
            Dispatcher.Invoke(() => {
                applicationData.UsersFriends.Add(profile);
                applicationData.UsersIncomingRequests.Remove(applicationData.UsersIncomingRequests.FirstOrDefault(u => u.Login == profile.Login));
                applicationData.UsersOutgoingRequests.Remove(applicationData.UsersOutgoingRequests.FirstOrDefault(u => u.Login == profile.Login));
            });

        }
        //Методы обработки ивента личных чатов
        private void PersonalChatCreate(object sender, PersonalChatInformation chat)
        {
            Dispatcher.Invoke(() => {
                applicationData.PersonalChatsInformation.Add(chat);
                applicationData.PersonalChats.Add(chat.ChatId, new());
            });
        }
        private void GroupChatCreate(object sender, GroupChatInformation chat)
        {
            Dispatcher.Invoke(() => {
                applicationData.GroupChatsInformation.Add(chat);
                applicationData.GroupChats.Add(chat.ChatId, new());
            });
        }

        private void MessageSent(object sender, MessageSentEventArgs args)
        {
            Dispatcher.Invoke(() =>
            {
                if(args.IsGroup)
                    ChatHelper.AddMessage(applicationData.GroupChats, args.Message, args.Index);
                else
                    ChatHelper.AddMessage(applicationData.PersonalChats, args.Message, args.Index);
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetProfileInformation();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) =>
            App.CloseWindow(this);
        private void MinimizeButton_Click(object sender, RoutedEventArgs e) =>
            App.MinimizeWindow(this);
        private void MaximizeButton_Click(object sender, RoutedEventArgs e) =>
            App.MaximizeWindow(this);

        private void MenuClick(object sender, MouseButtonEventArgs e) =>
            MenuClickHelper(sender);
        private void MenuClickHelper(object sender, PersonalChatInformation? ChatInform = null)
        {
            MenuGrid grid = (sender as MenuGrid)!;
            Grid parent;
            if (grid.Name != "Profile")
                parent = (Grid)VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(grid));
            else
                parent = (Grid)VisualTreeHelper.GetParent(grid);
            foreach (var child in parent.Children)
            {
                if (child is MenuGrid)
                {
                    MenuGrid menu = (child as MenuGrid)!;
                    menu.IsSelected = false;
                }
                else
                {
                    StackPanel panel = (child as StackPanel)!;
                    if (panel == null)
                        continue;
                    foreach (MenuGrid menu in panel.Children)
                    {
                        menu.IsSelected = false;
                    }
                }
            }
            grid.IsSelected = true;

            var typeOfGridContent = MainFrame.Content?.GetType();
            if (grid.Name == "Friends" && typeOfGridContent != typeof(FriendsMenuPage))
                MainFrame.Navigate(new FriendsMenuPage());
            if (grid.Name == "Chats" && typeOfGridContent != typeof(PersonalChatsListPage))
            {
                if (ChatInform is null)
                    MainFrame.Navigate(new PersonalChatsListPage());
                else
                    MainFrame.Navigate(new PersonalChatsListPage(ChatInform));
            }
            if (grid.Name == "Groups" && typeOfGridContent != typeof(GroupChatsPage))
                    MainFrame.Navigate(new GroupChatsPage());
            if (grid.Name == "Profile" && typeOfGridContent != typeof(ProfilePage))
            {
                ProfileFrame.Visibility = Visibility.Collapsed;
                MainFrame.Navigate(new ProfilePage());
            }
        }

        private async void SetProfileInformation()
        {
            await applicationData.LoadedTask;
            LoginLabel.Content = applicationData.UserProfile.Login;
            if (applicationData.UserProfile.ImageByte is null)
                return;
            var image = App.GetImageFromByteArray(applicationData.UserProfile.ImageByte);
            ProfileImage.Source = image;
            SetProfileImage(image);

        }

        private async void Logout(object sender, MouseButtonEventArgs e)
        {
            var response = await httpClient.PostAsync(App.HttpsStr + "/logout", null);
            httpClient.DefaultRequestHeaders.Authorization = null;
            var window = new InitialWindow();
            window.Show();
            this.Close();
        }

        internal void SetProfileImage(BitmapImage image) =>
            ProfileImage.Source = image;

        internal void OpenUserProfile(object sender, MouseButtonEventArgs e, ProfileInformation? profile = null)
        {
            ProfileInformation userProfile;
            if (profile is null)
            {
                Grid grid = sender as Grid;
                userProfile = grid.DataContext as ProfileInformation;
            }
            else
                userProfile = profile;
            if (ProfileFrame.Visibility == Visibility.Visible)
            {
                OtherUserProfilePage content = ProfileFrame.Content as OtherUserProfilePage;
                if (content.Profile.Login == userProfile.Login)
                    return;
            }
            ProfileFrame.Navigate(new OtherUserProfilePage(userProfile));
            ProfileFrame.Visibility = Visibility.Visible;
        }

        private void ProfileFrame_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ProfileFrameColumn.Width = ProfileFrame.Visibility == Visibility.Visible ? new GridLength(0.4, GridUnitType.Star) : new GridLength(0);
        }

        internal void GoToFriendPage() =>
            MenuClickHelper(Friends);

        internal async void GoToChat(object sender, ProfileInformation? profileInformation = null)
        {
            var button = (Button)sender;
            ProfileInformation profile;
            if (profileInformation is null)
                profile = button.DataContext as ProfileInformation;
            else
                profile = profileInformation;
            var response = await httpClient.GetAsync(App.HttpsStr + $"/personalchats/{profile.Login}");
            if (!response.IsSuccessStatusCode)
                return;
            int chatId = await response.Content.ReadFromJsonAsync<int>();
            PersonalChatInformation chat = new PersonalChatInformation { Login = profile.Login, ImageByte = profile.ImageByte, ChatId = chatId };
            MenuClickHelper(Chats, chat);
            if (applicationData.PersonalChatsInformation.FirstOrDefault(c => c.Login == profile.Login) is null)
                applicationData.PersonalChatsInformation.Add(chat);
            if (!applicationData.PersonalChats.ContainsKey(chatId))
                applicationData.PersonalChats.Add(chatId, new());
        }
    }
}
