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
        private MainWindow currentWindow;
        private readonly string NoFriendsWarn = "У тебя пока что нет ни одного друга :(";
        private readonly string NotFoundFilterWarn = "Нет друзей с таким логином";
        private ObservableCollection<ProfileInformation> usersFilterList = new ObservableCollection<ProfileInformation>();
        private ApplicationData applicationData;
        public AllFriendsPage()
        {
            InitializeComponent();
            httpClient = HttpClientSingleton.httpClient!;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            currentWindow = Window.GetWindow(this) as MainWindow;
            applicationData = currentWindow.applicationData;
            UsersListBox.ItemsSource = applicationData.UsersFriends;
            currentWindow.friendNotificationService.FriendAdded += FriendAdd;
            currentWindow.friendNotificationService.FriendDeleted += FriendDelete;
            CheckSource();
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            currentWindow.friendNotificationService.FriendAdded -= FriendAdd;
            currentWindow.friendNotificationService.FriendDeleted -= FriendDelete;
        }

        private void FriendAdd(object sender, ProfileInformation profile)
        {
            Dispatcher.Invoke(() =>
            {
                UserWarn.Visibility = Visibility.Collapsed;
                UsersListBox.Visibility = Visibility.Visible;
            });
        }
        private void FriendDelete(object sender, ProfileInformation profile)
        {
            Dispatcher.Invoke(() =>
            {
                if (applicationData.UsersFriends.Count == 0)
                {
                    UserWarn.Visibility = Visibility.Visible;
                    UserWarn.Text = NoFriendsWarn;
                    UsersListBox.Visibility = Visibility.Collapsed;
                }
            });
        }

        private void Image_Loaded(object sender, RoutedEventArgs e) => 
            FriendsMenuPage.Image_Loaded(sender, e);
        

        private async void DeleteFriend(object sender, RoutedEventArgs e)
        {
            var image = (Button)sender;
            var profile = image.DataContext as ProfileInformation;
            var response = await FriendsMenuPage.MakeRelationshipAction(RelationshipAction.Delete, profile.Login);
            if (response.IsSuccessStatusCode)
            {
                applicationData.UsersFriends.Remove(profile);
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
                UsersListBox.ItemsSource = applicationData.UsersFriends;
            }
            else
            {
                var filteredCollection = applicationData.UsersFriends.Where(u => Regex.IsMatch(u.Login.ToLower(), $"^.*{textBox.Text.ToLower()}.*$"));
                usersFilterList = new ObservableCollection<ProfileInformation>(filteredCollection);
                CheckFilter();
                UsersListBox.ItemsSource = usersFilterList;
            }
        }
        private void CheckSource()
        {
            if (applicationData.UsersFriends.Count == 0)
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
            currentWindow.OpenUserProfile(sender,e);
    }
}
