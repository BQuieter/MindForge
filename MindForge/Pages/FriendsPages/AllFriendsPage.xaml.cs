using MindForge;
using MindForgeClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;
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

namespace MindForgeClient.Pages.FriendsPages
{
    /// <summary>
    /// Логика взаимодействия для AllFriendsPage.xaml
    /// </summary>
    public partial class AllFriendsPage : Page
    {
        private HttpClient httpClient;
        ProfileInformation profileInformation;
        private readonly string NoFriendsWarn = "У тебя пока что нет ни одного друга :(";
        private readonly string NotFoundFilterWarn = "Нет друзей с таким логином";
        ObservableCollection<ProfileInformation> usersSource = new ObservableCollection<ProfileInformation>();
        ObservableCollection<ProfileInformation> usersFilterList = new ObservableCollection<ProfileInformation>();
        public AllFriendsPage()
        {
            InitializeComponent();
            httpClient = HttpClientSingleton.httpClient!;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Window currentWindow = Window.GetWindow(this);
            profileInformation = (ProfileInformation)currentWindow.Resources["Profile"];
            var friendsResponse = await httpClient.GetAsync(App.HttpsStr + "/friends/all");
            if (friendsResponse.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                ShowWarn(NoFriendsWarn);
                return;
            }
            var friends = await friendsResponse.Content.ReadFromJsonAsync<List<ProfileInformation>>();
            if (friends is not null && friends.Count > 0)
            {
                usersSource = new ObservableCollection<ProfileInformation>(friends);
                UsersListBox.ItemsSource = usersSource;
            }
            CheckSource();
    }

    private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            var image = (Image)sender;
            var profile = image.DataContext as ProfileInformation;
            if (profile!.ImageByte is not null)
                image.Source = App.GetImageFromByteArray(profile.ImageByte);
        }

        private async void DeleteFriend(object sender, RoutedEventArgs e)
        {
            var image = (Button)sender;
            var profile = image.DataContext as ProfileInformation;
            var response = await FriendsMenuPage.MakeRelationshipAction(RelationshipAction.Delete, profile.Login);
            if (response.IsSuccessStatusCode)
            {
                usersSource.Remove(profile);
                usersFilterList.Remove(profile);
                CheckSource();
                CheckFilter();
            }
        }

        private void FilterFriends(object sender, TextChangedEventArgs e)
        {
            App.WatermarkHelper(sender, e);
            TextBox textBox = (TextBox)sender;
            if (textBox.Text.Length == 0)
            {
                CheckSource();
                UsersListBox.ItemsSource = usersSource;
            }
            else
            {
                var filteredCollection = usersSource.Where(u => Regex.IsMatch(u.Login.ToLower(), $"^.*{textBox.Text.ToLower()}.*$"));
                usersFilterList = new ObservableCollection<ProfileInformation>(filteredCollection);
                CheckFilter();
                UsersListBox.ItemsSource = usersFilterList;
            }
        }
        private void CheckSource()
        {
            if (usersSource is null || usersSource.Count == 0)
                ShowWarn(NoFriendsWarn);
            else UserWarn.Visibility = Visibility.Collapsed;
        }

        private void CheckFilter()
        {
            if ( usersFilterList is null || usersFilterList.Count == 0)
                ShowWarn(NotFoundFilterWarn);
            else UserWarn.Visibility = Visibility.Collapsed;
        }

        private void ShowWarn(string warnText)
        {
            UserWarn.Text = warnText;
            UserWarn.Visibility = Visibility.Visible;
        }
    }
}
