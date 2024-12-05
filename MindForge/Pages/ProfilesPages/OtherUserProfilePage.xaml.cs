using MindForge;
using MindForgeClasses;
using System;
using System.Collections.Generic;
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

namespace MindForgeClient.Pages
{
    /// <summary>
    /// Логика взаимодействия для OtherUserProfilePage.xaml
    /// </summary>
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
            Description.Text = Profile.Description;
            ProfileImage.Source = App.GetImageFromByteArray(Profile.ImageByte);
            var response = await httpClient.GetAsync(App.HttpsStr + $"/professions/{Profile.Login}");
            if (!response.IsSuccessStatusCode) // ошибку обработай как нить
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
