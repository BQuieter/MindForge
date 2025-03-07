using MindForge;
using MindForgeClasses;
using MindForgeClient.Pages;
using MindForgeClient.Pages.Chats;
using MindForgeClient.Pages.Chats.Group;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MindForgeClient
{
    public partial class MainWindow : Window
    {
        internal ApplicationData applicationData;
        private static HttpClient httpClient;
        internal readonly IFriendNotificationService friendNotificationService;
        internal readonly IPersonalChatNotificationService personalChatNotificationService;
        internal readonly ICallsService callsService;
        public MainWindow(IFriendNotificationService friendNotificationService, IPersonalChatNotificationService personalChatNotificationService, ICallsService callsService)
        {
            this.friendNotificationService = friendNotificationService;
            this.personalChatNotificationService = personalChatNotificationService;
            this.callsService = callsService;
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
            personalChatNotificationService.MemberAdded += MemberAdd!;
            personalChatNotificationService.MemberDeleted += MemberDelete!;
            personalChatNotificationService.YouDeleted += YouDelete!;
            callsService.UserJoined += UserJoin!;
            callsService.UserLeaved += UserLeave!;
            friendNotificationService.StartAsync();
            personalChatNotificationService.StartAsync();
            callsService.StartAsync();
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
        private void MemberAdd(object sender, MemberAddEventArgs args)
        {
            Dispatcher.Invoke(() => {
                var chat = applicationData.GroupChatsInformation.FirstOrDefault(c => c.ChatId == args.ChatId);
                if (chat is null)
                    return;
                foreach (var member in args.Users)
                    chat.Members.Add(member);
            });
        }

        private void MemberDelete(object sender, MemberEventArgs args)
        {
            Dispatcher.Invoke(() => {
                var chat = applicationData.GroupChatsInformation.FirstOrDefault(c => c.ChatId == args.ChatId);
                if (chat is null)
                    return;
                foreach (var member in chat.Members.ToList())
                    if (member.Login == args.User.Login)
                        chat.Members.Remove(member);
            });
        }

        private void YouDelete(object sender, int chatId)
        {
            Dispatcher.Invoke(() => {
                applicationData.GroupChatsInformation.Remove(applicationData.GroupChatsInformation.FirstOrDefault(c => c.ChatId == chatId));
                applicationData.GroupChats.Remove(chatId);
                if (MainFrame.Content is  GroupChatsPage page)
                {
                    if (page.CurrentChatId == chatId)
                        page.CloseChat();
                }
            });
        }
        //calls
        private void UserJoin(object sender, MemberEventArgs args)
        {
            Dispatcher.Invoke(() => {
                applicationData.CallsParticipants[args.ChatId].Add(args.User);
            });
        }

        private void UserLeave(object sender, MemberEventArgs args)
        {
            Dispatcher.Invoke(() => {
                applicationData.CallsParticipants[args.ChatId].Remove(applicationData.CallsParticipants[args.ChatId].FirstOrDefault(p => p.Login == args.User.Login));
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
            CallHelper.LeaveCall();
            httpClient.DefaultRequestHeaders.Authorization = null;
            var window = new InitialWindow();
            window.Show();
            this.Close();
        }

        internal void SetProfileImage(BitmapImage image) =>
            ProfileImage.Source = image;

        internal void OpenProfileFrame(object sender, ProfileInformation? profile = null)
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

        internal async void OpenProfileFrame(PersonalChatInformation chat)
        {
            var response = await httpClient.GetAsync(App.HttpsStr + $"/profile/{chat.Login}");
            if (response is null || !response.IsSuccessStatusCode)
                return;
            ProfileInformation userProfile = await response.Content.ReadFromJsonAsync<ProfileInformation>();
          
            if (ProfileFrame.Visibility == Visibility.Visible)
            {
                OtherUserProfilePage content = ProfileFrame.Content as OtherUserProfilePage;
                    if (content.Profile.Login == userProfile.Login)
                    return;
            }
            ProfileFrame.Navigate(new OtherUserProfilePage(userProfile));
            ProfileFrame.Visibility = Visibility.Visible;
        }

        internal async void OpenProfileFrame(GroupChatInformation chat)
        {
            if (ProfileFrame.Visibility == Visibility.Visible)
            {
                GroupChatInformPage content = ProfileFrame.Content as GroupChatInformPage;
                if (content.GroupName == chat.Name)
                    return;
            }
            ProfileFrame.Navigate(new GroupChatInformPage(chat));
            ProfileFrame.Visibility = Visibility.Visible;
        }

        internal void CloseProfileFrame()
        {
            ProfileFrame.Navigate(null);
            ProfileFrame.Visibility = Visibility.Collapsed;
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

        private void Window_Closed(object sender, EventArgs e)
        {
            CallHelper.LeaveCall();
        }
    }
}
