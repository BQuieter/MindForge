using MindForge;
using MindForgeClasses;
using MindForgeClient.Pages.Call;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MindForgeClient.Pages.Chats
{
    public partial class ChatPage : Page
    {
        private static double? verticalOffset = null;
        public static int? CallChatId = null;
        private bool isCall = false;
        private MainWindow currentWindow;
        private HttpClient httpClient;
        private ApplicationData applicationData;
        private BitmapImage chatPartnerBitmapImage;
        private BitmapImage userBitmapImage;
        private int chatId;
        private PersonalChatInformation personalChatInformation;
        private GroupChatInformation groupChatInformation;
        private event EventHandler<int> CallLeaveEvent;
        private Dictionary<string, BitmapImage> avatars = new();
        public ChatPage(PersonalChatInformation chatInformation)
        {
            InitializeComponent();
            this.personalChatInformation = chatInformation;
            chatId = personalChatInformation.ChatId;
            httpClient = HttpClientSingleton.httpClient;
        }

        public ChatPage(GroupChatInformation chatInformation)
        {
            InitializeComponent();
            this.groupChatInformation = chatInformation;
            chatId = groupChatInformation.ChatId;
            httpClient = HttpClientSingleton.httpClient;
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            currentWindow = Window.GetWindow(this) as MainWindow;
            applicationData = currentWindow.applicationData;
            CallLeaveEvent += CallLeave;
            currentWindow.callsService.UserJoined += UserJoin!;
            currentWindow.callsService.UserLeaved += UserLeave!;
            GroupMessages();
            if (personalChatInformation is not null)
            {
                avatars.Add(personalChatInformation.Login, App.GetImageFromByteArray(personalChatInformation.ImageByte));
                avatars.Add(applicationData.UserProfile.Login, App.GetImageFromByteArray(applicationData.UserProfile.ImageByte));
                ChatPartnerImage.Source = avatars[personalChatInformation.Login];
                ChatPartnerLogin.Text = personalChatInformation.Login;
            }
            else
            {
                foreach (var profile in groupChatInformation.Members)
                    avatars.Add(profile.Login, App.GetImageFromByteArray(profile.ImageByte));
                ChatPartnerImage.Source = App.GetImageFromByteArray(groupChatInformation.ImageByte);
                ChatPartnerLogin.Text = groupChatInformation.Name;
            }
            currentWindow.personalChatNotificationService.MessageSent += MessageSent!;
            var response = await httpClient.GetAsync(App.HttpsStr + $"/call/get/{chatId}");
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                isCall = false;
                return;
            }
            else
            {
                isCall = true;
                MainFrame.Visibility = Visibility.Visible;
                if (applicationData.CallsParticipants[chatId].Count == 0)
                {
                    ObservableCollection<ProfileInformation> participants = await response.Content.ReadFromJsonAsync<ObservableCollection<ProfileInformation>>();
                    applicationData.CallsParticipants[chatId] = participants;
                }
                bool joined = CallHelper.InCall && CallHelper.ChatId == chatId ? true : false;
                MainFrame.Navigate(new CallPage(chatId, joined, CallLeaveEvent));
            }
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            currentWindow.personalChatNotificationService.MessageSent -= MessageSent!;
            currentWindow.CloseProfileFrame();
            CallLeaveEvent -= CallLeave;
            currentWindow.callsService.UserJoined -= UserJoin!;
            currentWindow.callsService.UserLeaved -= UserLeave!;
        }

        private void UserJoin(object sender, MemberEventArgs args)
        {
            Dispatcher.Invoke(() => {
                if (args.ChatId == chatId && applicationData.CallsParticipants[args.ChatId].Count == 1)
                {
                    MainFrame.Navigate(new CallPage(chatId, false, CallLeaveEvent));
                    MainFrame.Visibility = Visibility.Visible;
                }
            });
        }

        private void UserLeave(object sender, MemberEventArgs args)
        {
            Dispatcher.Invoke(() => {
                if (applicationData.CallsParticipants[args.ChatId].Count == 0)
                {
                    MainFrame.Navigate(null);
                    MainFrame.Visibility = Visibility.Collapsed;
                }
            });
        }

        private void CallLeave(object sender, int count)
        {
            if (count < 1)
            {
                MainFrame.Navigate(null);
                MainFrame.Visibility = Visibility.Collapsed;
            }
        }

        private void MessageSent(object sender, MessageSentEventArgs args)
        {
            Dispatcher.Invoke(() => {
                if (ScrollViewer.VerticalOffset + ScrollViewer.ViewportHeight >= ScrollViewer.ExtentHeight)
                    ScrollViewer.ScrollToBottom();
            });
        }
        private async void GroupMessages()
        {
            var response = await httpClient.GetAsync(App.HttpsStr + $"/messages/{chatId}");
            if (!response.IsSuccessStatusCode)
                return;
            var messages = await response.Content.ReadFromJsonAsync<List<MessageInformation>>();
            if (verticalOffset is null)
                ScrollViewer.ScrollToBottom();
            else
                ScrollViewer.ScrollToVerticalOffset((double)verticalOffset);
            if (messages is null || messages.Count == 0)
            {
                if (personalChatInformation is not null)
                    MessageList.ItemsSource = applicationData.PersonalChats[chatId];
                else
                    MessageList.ItemsSource = applicationData.GroupChats[chatId];
                return;
            }
            ObservableCollection<MessageGroup> groups;
            if (personalChatInformation is not null)
                 groups = applicationData.PersonalChats[chatId];
            else
                groups = applicationData.GroupChats[chatId];

            if (groups.Count < 1)
            {
                messages.Reverse();
                MessageGroup currentGroup = new(messages[0].SenderName, messages[0]);
                for (int i = 0; i < messages.Count; i++)
                {
                    if (personalChatInformation is not null)
                        ChatHelper.AddMessage(applicationData.PersonalChats, messages[i], chatId);
                    else
                        ChatHelper.AddMessage(applicationData.GroupChats, messages[i], chatId);
                    /*if (messages[i].SenderName == currentGroup.SenderName)
                    {
                        currentGroup.Messages.Add(messages[i]);
                    }
                    else
                    {
                        currentGroup.Messages = new ObservableCollection<MessageInformation>(currentGroup.Messages);
                        groups.Add(currentGroup);
                        currentGroup = new(messages[i].SenderName, messages[i]);
                    }*/
                }
                //groups.Add(currentGroup);
                ScrollViewer.ScrollToBottom();
                if (personalChatInformation is not null)
                    applicationData.PersonalChats[chatId] = new ObservableCollection<MessageGroup>(applicationData.PersonalChats[chatId]);
                else
                    applicationData.GroupChats[chatId] = new ObservableCollection<MessageGroup>(applicationData.GroupChats[chatId]);

            }
            if (personalChatInformation is not null)
                MessageList.ItemsSource = applicationData.PersonalChats[chatId];
            else
                MessageList.ItemsSource = applicationData.GroupChats[chatId];
        }
        private void MessageTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            App.WatermarkHelper(sender, e);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
            {
                e.Handled = true;
                DoSomethingOnEnter();
            }
        }

        private void DoSomethingOnEnter() =>
            SendMessage();

        private void SendMessage_Click(object sender, RoutedEventArgs e) =>
            SendMessage();

        private async void SendMessage()
        {
            if (MessageTextBox.Text == String.Empty)
                return;
            MessageInformation message = new MessageInformation() {Message = MessageTextBox.Text, SenderName = applicationData.UserProfile.Login, Time = DateTime.Now.ToShortTimeString(), Date = DateTime.Now.Day, Month = DateTime.Now.Month, Year = DateTime.Now.Year  };
            if (personalChatInformation is not null)
                ChatHelper.AddMessage(applicationData.PersonalChats, message, chatId);
            else
            {
                ChatHelper.AddMessage(applicationData.GroupChats, message, chatId);
                message.GroupName = groupChatInformation.Name;
            }
            MessageTextBox.Text = String.Empty;
            if (MessageList.ItemsSource is null && (applicationData.PersonalChats.TryGetValue(chatId, out ObservableCollection<MessageGroup> personalChat) | applicationData.GroupChats.TryGetValue(chatId, out ObservableCollection<MessageGroup> groupChat)))
                MessageList.ItemsSource = personalChat is null ? groupChat : personalChat;

            if (ScrollViewer.VerticalOffset + ScrollViewer.ViewportHeight >= ScrollViewer.ExtentHeight)
                ScrollViewer.ScrollToBottom();
            await httpClient.PostAsJsonAsync<MessageInformation>(App.HttpsStr + $"/chats/message/{chatId}",message);
        }

        private async void Image_Loaded(object sender, RoutedEventArgs e)
        {
            Image image = sender as Image;
            MessageGroup context = image.DataContext as MessageGroup;
            if (context is not null && !avatars.ContainsKey(context!.SenderName))
            {
                var response = await httpClient.GetAsync(App.HttpsStr + $"/profile/{context.SenderName}");
                if (!response.IsSuccessStatusCode)
                    return;
                var profile = await response.Content.ReadFromJsonAsync<ProfileInformation>();
                avatars.Add(context.SenderName, App.GetImageFromByteArray(profile.ImageByte));
            }
            image.Source = avatars[context.SenderName];
        }

        private void ScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = sender as ScrollViewer;

            double newVerticalOffset = scrollViewer.VerticalOffset - e.Delta;
            newVerticalOffset = Math.Max(0, newVerticalOffset);
            newVerticalOffset = Math.Min(scrollViewer.ScrollableHeight, newVerticalOffset);
            scrollViewer.ScrollToVerticalOffset(newVerticalOffset);
            verticalOffset = newVerticalOffset;
            e.Handled = true;
        }

        private void OpenInformation(object sender, MouseButtonEventArgs e)
        {
            if (personalChatInformation is not null)
                currentWindow.OpenProfileFrame(personalChatInformation);
            else
            {
                currentWindow.CloseProfileFrame();
                currentWindow.OpenProfileFrame(groupChatInformation);
            }

        }

        private void Call_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content is not null)
                return;
            MainFrame.Visibility = Visibility.Visible;
            var reponse = httpClient.GetAsync(App.HttpsStr + $"/call/create/{chatId}");
            var profiles = new ObservableCollection<ProfileInformation> { applicationData.UserProfile };
            CallHelper.Participants = profiles;
            MainFrame.Navigate(new CallPage(chatId, true, CallLeaveEvent));
            if (applicationData.CallsParticipants is null || !applicationData.CallsParticipants.ContainsKey(chatId))
                applicationData.CallsParticipants.Add(chatId, new());
            applicationData.CallsParticipants[chatId].Add(applicationData.UserProfile);
        }

        private void DateString_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = (sender as TextBlock)!;
            if (string.IsNullOrEmpty(textBlock.Text))
                textBlock.Visibility = Visibility.Collapsed;
        }
    }
}
