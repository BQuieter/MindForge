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
    /// Логика взаимодействия для IncomingRequestsPage.xaml
    /// </summary>
    public partial class IncomingRequestsPage : Page
    {
        private HttpClient httpClient;
        MainWindow currentWindow;
        private readonly string NoFriendsWarn = "У тебя нет входящих запросов";
        private readonly string NotFoundFilterWarn = "Нет пользователей с таким логином";
        private ObservableCollection<ProfileInformation> usersFilterList = new ObservableCollection<ProfileInformation>();
        private ApplicationData applicationData;
        public IncomingRequestsPage()
        {
            InitializeComponent();
            httpClient = HttpClientSingleton.httpClient!;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            currentWindow = Window.GetWindow(this) as MainWindow;
            applicationData = currentWindow.applicationData;
            UsersListBox.ItemsSource = applicationData.UsersIncomingRequests;
            currentWindow.friendNotificationService.FriendRequestReceived += RequestAdd;
            currentWindow.friendNotificationService.FriendRequestDeleted += RequestDelete;
            CheckSource();
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            currentWindow.friendNotificationService.FriendRequestReceived -= RequestAdd;
            currentWindow.friendNotificationService.FriendRequestDeleted -= RequestDelete;
        }

        private void RequestAdd(object sender, ProfileInformation profile)
        {
            Dispatcher.Invoke(() =>
            {
                UsersListBox.Visibility = Visibility.Visible;
                UserWarn.Visibility = Visibility.Collapsed;
            });
        }
        private void RequestDelete(object sender, ProfileInformation profile)
        {
            Dispatcher.Invoke(() =>
            {
                if (applicationData.UsersIncomingRequests.Count == 0)
                {
                    UsersListBox.Visibility = Visibility.Collapsed;
                    UserWarn.Text = NoFriendsWarn;
                    UserWarn.Visibility = Visibility.Visible;
                }
            });
        }
        private void Image_Loaded(object sender, RoutedEventArgs e) =>
            FriendsMenuPage.Image_Loaded(sender, e);

        private async void ApplyRequest(object sender, RoutedEventArgs e)
        {
            var image = (Button)sender;
            var profile = image.DataContext as ProfileInformation;
            var response = await FriendsMenuPage.MakeRelationshipAction(RelationshipAction.Apply, profile.Login);
            if (response.IsSuccessStatusCode)
            {
                //Тут добавь чтоб удалялось по сигнал р
                applicationData.UsersIncomingRequests.Remove(profile);
                applicationData.UsersFriends.Add(profile);
                usersFilterList.Remove(profile);
                CheckSource();
                CheckFilter();
            }
        }

        private async void RejectRequest(object sender, RoutedEventArgs e)
        {
            var image = (Button)sender;
            var profile = image.DataContext as ProfileInformation;
            var response = await FriendsMenuPage.MakeRelationshipAction(RelationshipAction.Delete, profile.Login);
            if (response.IsSuccessStatusCode)
            {
                applicationData.UsersIncomingRequests.Remove(profile);
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
                UsersListBox.ItemsSource = applicationData.UsersIncomingRequests;
            }
            else
            {
                var filteredCollection = applicationData.UsersIncomingRequests.Where(u => Regex.IsMatch(u.Login.ToLower(), $"^.*{textBox.Text.ToLower()}.*$"));
                usersFilterList = new ObservableCollection<ProfileInformation>(filteredCollection);
                CheckFilter();
                UsersListBox.ItemsSource = usersFilterList;
            }
        }
        private void CheckSource()
        {
            if (applicationData.UsersIncomingRequests.Count == 0)
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
        private void OpenProfile(object sender, MouseButtonEventArgs e) =>
            currentWindow.OpenUserProfile(sender, e);

        private void GoToChat(object sender, RoutedEventArgs e) =>
            currentWindow.GoToChat(sender);
    }
}
