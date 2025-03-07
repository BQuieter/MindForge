using MindForge;
using MindForgeClasses;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace MindForgeClient.Pages
{
    public partial class OtherUserProfilePage : Page
    {
        private HttpClient httpClient;
        internal ProfileInformation Profile { get; private set; }
        public OtherUserProfilePage(ProfileInformation profile)
        {
            InitializeComponent();
            Profile = profile;
            httpClient = HttpClientSingleton.httpClient;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoginTextBlock.Text = Profile.Login;
            Description.Text = Profile.Description is not null && Profile.Description.Length > 0 ? Profile.Description : "Пользователь ничего не указал о себе";
            ProfileImage.Source = App.GetImageFromByteArray(Profile.ImageByte);
            var response = await httpClient.GetAsync(App.HttpsStr + $"/professions/{Profile.Login}");
            if (!response.IsSuccessStatusCode) 
                return;
            ProfessionListBox.ItemsSource = await response.Content.ReadFromJsonAsync<List<ProfessionInformation>>();

        }

        private void ClosePage(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
            mainWindow.ProfileFrame.Visibility = Visibility.Collapsed;
        }
    }
}
