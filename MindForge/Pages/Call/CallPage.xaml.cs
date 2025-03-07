using MindForge;
using MindForgeClasses;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MindForgeClient.Pages.Call
{
    public partial class CallPage : Page
    {
        private HttpClient httpClient;
        private MainWindow currentWindow;
        private ApplicationData applicationData;
        private EventHandler<int> leaveEvent;
        private readonly bool joined;
        private int chatId;
        private readonly string MuteToolTip = "Выключить микрофон";
        private readonly string UnmuteToolTip = "Включить микрофон";
        //-------------------
        internal static AudioStreamer audioStreamer;
        internal static AudioPlayer audioPlayer;
        public CallPage(int chatId, bool joined, EventHandler<int> leaveEvent)
        {
            InitializeComponent();
            httpClient = HttpClientSingleton.httpClient;
            this.leaveEvent = leaveEvent;
            this.chatId = chatId;
            this.joined = joined;

        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            currentWindow = Window.GetWindow(this) as MainWindow;
            applicationData = currentWindow.applicationData;
            ParticipantItemsControl.Items.Clear();
            ParticipantItemsControl.ItemsSource = applicationData.CallsParticipants[chatId];
            if (audioStreamer is null)
                audioStreamer = new(currentWindow.callsService.GetConnection());
            if (audioPlayer is null)
                audioPlayer = new(currentWindow.callsService.GetConnection());
            if (joined)
                Join();
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
        }
        private void Leave()
        {
            JoinButton.Visibility = Visibility.Visible;
            InCallGrid.Visibility = Visibility.Collapsed;
            CallHelper.InCall = false;
            applicationData.CallsParticipants[chatId].Remove(applicationData.CallsParticipants[chatId].FirstOrDefault(p => p.Login == applicationData.UserProfile.Login));
            leaveEvent?.Invoke(this, applicationData.CallsParticipants[chatId].Count);
            currentWindow.callsService.LeaveGroup(chatId);
            audioStreamer.StopStreaming();
            audioPlayer.StopPlaying();
        }
        private async void Join()
        {
            if (CallHelper.isMuted)
            {
                MuteImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/MenuIcons/Call/UnmuteMicrophone.png"));
                MuteButton.ToolTip = UnmuteToolTip;
            }
            
            JoinButton.Visibility = Visibility.Collapsed;
            InCallGrid.Visibility = Visibility.Visible;
            CallHelper.ChatId = chatId;
            CallHelper.InCall = true;
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
            applicationData.CallsParticipants[chatId].Add(applicationData.UserProfile);
            Join();
        }

        private void Leave_Click(object sender, RoutedEventArgs e)
        {
            httpClient.GetAsync(App.HttpsStr + $"/call/leave/{chatId}");
            Leave();
        }

        private void Mute_Unmute_Click(object sender, RoutedEventArgs e)
        {
            if (CallHelper.isMuted)
            {
                CallHelper.isMuted = false;
                MuteImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/MenuIcons/Call/MuteMicrophone.png"));
                MuteButton.ToolTip = MuteToolTip;
            }
            else
            {
                CallHelper.isMuted = true;
                MuteImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/MenuIcons/Call/UnmuteMicrophone.png"));
                MuteButton.ToolTip = UnmuteToolTip;
            }
        }
    }
}
