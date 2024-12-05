using MindForge;
using MindForgeClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MindForgeClient.Pages.Chats
{
    /// <summary>
    /// Логика взаимодействия для ChatPage.xaml
    /// </summary>
    public partial class ChatPage : Page
    {
        private MainWindow currentWindow;
        private HttpClient httpClient;
        private ApplicationData applicationData;
        private BitmapImage chatPartnerBitmapImage;
        private BitmapImage userBitmapImage;
        private PersonalChatInformation chatInformation;
        private Dictionary<string, BitmapImage> avatars = new();
        public ChatPage(PersonalChatInformation chatInformation)
        {
            InitializeComponent();
            this.chatInformation = chatInformation;
            httpClient = HttpClientSingleton.httpClient;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            currentWindow = Window.GetWindow(this) as MainWindow;
            applicationData = currentWindow.applicationData;
            GroupMessages();
            //
            avatars.Add(chatInformation.Login, App.GetImageFromByteArray(chatInformation.ImageByte));
            avatars.Add(applicationData.UserProfile.Login, App.GetImageFromByteArray(applicationData.UserProfile.ImageByte));
            ChatPartnerImage.Source = avatars[chatInformation.Login];
            ChatPartnerLogin.Text = chatInformation.Login;
            currentWindow.personalChatNotificationService.MessageSent += MessageSent!;
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            currentWindow.personalChatNotificationService.MessageSent -= MessageSent!;

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
            var response = await httpClient.GetAsync(App.HttpsStr + $"/personalchats/messages/{chatInformation.ChatId}");
            if (!response.IsSuccessStatusCode)
                return;
            var messages = await response.Content.ReadFromJsonAsync<List<MessageInformation>>();
            if (messages is null || messages.Count == 0)
            {
                MessageList.ItemsSource = applicationData.PersonalChats[chatInformation.ChatId];
                return;
            }
            ObservableCollection<MessageGroup> groups = applicationData.PersonalChats[chatInformation.ChatId];
            if (groups.Count < 1)
            {
                MessageGroup currentGroup = new(messages[0].SenderName, messages[0]);
                for (int i = 1; i < messages.Count; i++)
                {
                    if (messages[i].SenderName == currentGroup.SenderName)
                    {
                        currentGroup.Messages.Add(messages[i]);
                    }
                    else
                    {
                        currentGroup.Messages = new ObservableCollection<MessageInformation>(currentGroup.Messages.Reverse());
                        groups.Add(currentGroup);
                        currentGroup = new(messages[i].SenderName, messages[i]);
                    }
                }
                groups.Add(currentGroup);
                ScrollViewer.ScrollToBottom();

                applicationData.PersonalChats[chatInformation.ChatId] = new ObservableCollection<MessageGroup>(applicationData.PersonalChats[chatInformation.ChatId].Reverse());
            }
            MessageList.ItemsSource = applicationData.PersonalChats[chatInformation.ChatId];
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


        private void DoSomethingOnEnter()
        {
            SendMessage();
        }

        private void SendMessage_Click(object sender, RoutedEventArgs e) =>
            SendMessage();

        private async void SendMessage()
        {
            if (MessageTextBox.Text == String.Empty)
                return;
            MessageInformation message = new MessageInformation() {Message = MessageTextBox.Text, SenderName = applicationData.UserProfile.Login, DateTime = DateTime.Now.ToShortTimeString() };
            ChatHelper.AddMessage(applicationData, message, chatInformation.ChatId);
            MessageTextBox.Text = String.Empty;
            if (MessageList.ItemsSource is null && applicationData.PersonalChats.TryGetValue(chatInformation.ChatId, out ObservableCollection<MessageGroup> chat))
                MessageList.ItemsSource = chat;
            if (ScrollViewer.VerticalOffset + ScrollViewer.ViewportHeight >= ScrollViewer.ExtentHeight)
                ScrollViewer.ScrollToBottom();
            await httpClient.PostAsJsonAsync<MessageInformation>(App.HttpsStr + $"/chats/message/{chatInformation.ChatId}",message);
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            Image image = sender as Image;
            var context = image.DataContext as MessageGroup;
            image.Source = avatars[context.SenderName];
        }

        private void ScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = sender as ScrollViewer;

            double newVerticalOffset = scrollViewer.VerticalOffset - e.Delta;
            newVerticalOffset = Math.Max(0, newVerticalOffset);
            newVerticalOffset = Math.Min(scrollViewer.ScrollableHeight, newVerticalOffset);
            scrollViewer.ScrollToVerticalOffset(newVerticalOffset);
            e.Handled = true;
        }
    }
}
