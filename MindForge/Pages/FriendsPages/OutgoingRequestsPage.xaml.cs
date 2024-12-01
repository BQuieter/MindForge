using MindForge;
using MindForgeClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
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
    /// Логика взаимодействия для OutgoingRequestsPage.xaml
    /// </summary>
    public partial class OutgoingRequestsPage : Page
    {

        private HttpClient httpClient;
        MainWindow currentWindow;
        private readonly string NoFriendsWarn = "У тебя нет исходящих запросов";
        private readonly string NotFoundFilterWarn = "Нет пользователей с таким логином";
        private ObservableCollection<ProfileInformation> usersFilterList = new ObservableCollection<ProfileInformation>();
        private ApplicationData applicationData;
        public OutgoingRequestsPage()
        {
            InitializeComponent();
            httpClient = HttpClientSingleton.httpClient!;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            currentWindow = Window.GetWindow(this) as MainWindow;
            applicationData = currentWindow.applicationData;
            UsersListBox.ItemsSource = applicationData.UsersOutgoingRequests;
            currentWindow.friendNotificationService.FriendRequestRejected += RequestReject;
            CheckSource();
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            currentWindow.friendNotificationService.FriendRequestRejected -= RequestReject;
        }

        private void RequestReject(object sender, ProfileInformation profile)
        {
            Dispatcher.Invoke(() =>
            {
                if (applicationData.UsersOutgoingRequests.Count == 0)
                {
                    UserWarn.Visibility = Visibility.Visible;
                    UsersListBox.Visibility = Visibility.Collapsed;
                }
            });
        }
        
        private void Image_Loaded(object sender, RoutedEventArgs e) =>
            FriendsMenuPage.Image_Loaded(sender, e);

        private async void DeleteRequest(object sender, RoutedEventArgs e)
        {
            var image = (Button)sender;
            var profile = image.DataContext as ProfileInformation;
            var response = await FriendsMenuPage.MakeRelationshipAction(RelationshipAction.Delete, profile.Login);
            if (response.IsSuccessStatusCode)
            {
                applicationData.UsersOutgoingRequests.Remove(profile);
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
                UsersListBox.ItemsSource = applicationData.UsersOutgoingRequests;
            }
            else
            {
                var filteredCollection = applicationData.UsersOutgoingRequests.Where(u => Regex.IsMatch(u.Login.ToLower(), $"^.*{textBox.Text.ToLower()}.*$"));
                usersFilterList = new ObservableCollection<ProfileInformation>(filteredCollection);
                CheckFilter();
                UsersListBox.ItemsSource = usersFilterList;
            }
        }
        private void CheckSource()
        {
            if (applicationData.UsersOutgoingRequests.Count == 0)
                ShowWarn(NoFriendsWarn);
            else UserWarn.Visibility = Visibility.Collapsed;
        }

        private void CheckFilter()
        {
            if (LoginTextBox.Text.Length == 0)
                return;
            if (usersFilterList is null || usersFilterList.Count == 0)
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
