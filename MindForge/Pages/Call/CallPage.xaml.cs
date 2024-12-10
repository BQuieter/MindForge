using MindForge;
using MindForgeClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Printing;
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

namespace MindForgeClient.Pages.Call
{
    /// <summary>
    /// Логика взаимодействия для CallPage.xaml
    /// </summary>
    public partial class CallPage : Page
    {
        private HttpClient httpClient;
        private MainWindow currentWindow;
        private ApplicationData applicationData;
        private ObservableCollection<ProfileInformation> participants;
        private EventHandler<int> leaveEvent;
        private readonly bool joined;
        private int chatId;
        //-------------------
        internal static AudioStreamer audioStreamer;
        internal static AudioPlayer audioPlayer;
        public CallPage(ObservableCollection<ProfileInformation> participants, bool joined,int chatId, EventHandler<int> leaveEvent)
        {
            InitializeComponent();
            httpClient = HttpClientSingleton.httpClient;
            this.leaveEvent = leaveEvent;
            this.chatId = chatId;
            this.participants = participants;
            this.joined = joined;

        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ParticipantItemsControl.ItemsSource = participants;
            currentWindow = Window.GetWindow(this) as MainWindow;
            applicationData = currentWindow.applicationData;
            if (audioStreamer is null)
                audioStreamer = new(currentWindow.callsService.GetConnection());
            if (audioPlayer is null)
                audioPlayer = new(currentWindow.callsService.GetConnection());
            currentWindow.callsService.UserJoined += UserJoin!;
            currentWindow.callsService.UserLeaved += UserLeave!;
            if (joined)
                Join();
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            currentWindow.callsService.UserJoined -= UserJoin!;
            currentWindow.callsService.UserLeaved -= UserLeave!;
        }
        private void Leave()
        {
            JoinButton.Visibility = Visibility.Visible;
            InCallGrid.Visibility = Visibility.Collapsed;
            CallHelper.InCall = false;
            participants.Remove(applicationData.UserProfile);
            leaveEvent?.Invoke(this, participants.Count);
            currentWindow.callsService.LeaveGroup(chatId);
            audioStreamer.StopStreaming();
            audioPlayer.StopPlaying();
        }
        private async void Join()
        {
            JoinButton.Visibility = Visibility.Collapsed;
            InCallGrid.Visibility = Visibility.Visible;
            CallHelper.ChatId = chatId;
            CallHelper.InCall = true;
            CallHelper.Participants = participants;
            currentWindow.callsService.JoinGroup(chatId);
            await audioStreamer.StartStreamingAsync(chatId);
            await audioPlayer.StartPlayingAsync();
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            Image image = sender as Image;
            ProfileInformation information = image.DataContext as ProfileInformation;
            image.Source = App.GetImageFromByteArray(information.ImageByte);
        }

        private void JoinButton_Click(object sender, RoutedEventArgs e)
        {
            httpClient.GetAsync(App.HttpsStr + $"/call/join/{chatId}");
            Join();
        }

        private void Leave_Click(object sender, RoutedEventArgs e)
        {
            httpClient.GetAsync(App.HttpsStr + $"/call/leave/{chatId}");
            Leave();
        }

        private void UserJoin(object sender, MemberEventArgs args)
        {
            Dispatcher.Invoke(() => {
                if (args.ChatId != CallHelper.ChatId && args.ChatId == chatId)
                    participants.Add(args.User);
            });
        }

        private void UserLeave(object sender, MemberEventArgs args)
        {
            Dispatcher.Invoke(() => {
                if (args.ChatId != CallHelper.ChatId && args.ChatId == chatId)
                    participants.Remove(args.User);
            });
        }
    }
}
