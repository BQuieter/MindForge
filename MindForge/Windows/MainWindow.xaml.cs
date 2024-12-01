using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using MindForge;
using MindForgeClasses;
using MindForgeClient.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.IO;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Shell;

namespace MindForgeClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal ApplicationData applicationData;
        private static HttpClient httpClient;
        internal readonly IFriendNotificationService friendNotificationService;
        public MainWindow(IFriendNotificationService friendNotificationService)
        {
            this.friendNotificationService = friendNotificationService;
            InitializeComponent();
            this.BorderThickness = SystemParametersFix.WindowResizeBorderThickness;
            httpClient = HttpClientSingleton.httpClient!;
            applicationData = new();
            friendNotificationService.FriendRequestReceived += FriendRequestReceive!;
            friendNotificationService.FriendRequestRejected += FriendRequestReject!;
            friendNotificationService.FriendDeleted += FriendDelete!;
            friendNotificationService.FriendAdded += FriendAdd!;
            friendNotificationService.FriendRequestDeleted += FriendRequestDeleted!;
            friendNotificationService.StartAsync();
        }
        //Методы обработки ивента прихода данных друзей
        private void FriendRequestReceive(object sender, ProfileInformation profile)
        {
            Dispatcher.Invoke(() =>
                applicationData.UsersIncomingRequests.Add(profile));
        }
        private void FriendRequestReject(object sender, ProfileInformation profile)
        {
            Dispatcher.Invoke(() =>
                applicationData.UsersOutgoingRequests.Remove(applicationData.UsersOutgoingRequests.FirstOrDefault(u => u.Login == profile.Login))
            );
        }
        private void FriendRequestDeleted(object sender, ProfileInformation profile)
        {
            Dispatcher.Invoke(() =>
                applicationData.UsersIncomingRequests.Remove(applicationData.UsersIncomingRequests.FirstOrDefault(u => u.Login == profile.Login))
            );
        }
        private void FriendDelete(object sender, ProfileInformation profile)
        {
            Dispatcher.Invoke(() => 
                applicationData.UsersFriends.Remove(applicationData.UsersFriends.FirstOrDefault(u => u.Login == profile.Login))
            );

        }
        private void FriendAdd(object sender, ProfileInformation profile)
        {
            Dispatcher.Invoke(() => {
                applicationData.UsersFriends.Add(profile);
                applicationData.UsersIncomingRequests.Remove(applicationData.UsersIncomingRequests.FirstOrDefault(u => u.Login == profile.Login));
                applicationData.UsersOutgoingRequests.Remove(applicationData.UsersOutgoingRequests.FirstOrDefault(u => u.Login == profile.Login));
            });

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetProfileInformation();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) =>
            App.CloseWindow(this);
        private void MinimizeButton_Click(object sender, RoutedEventArgs e) =>
            App.MinimizeWindow(this);
        private void MaximizeButton_Click(object sender, RoutedEventArgs e) =>
            App.MaximizeWindow(this);

        private void MenuClick(object sender, MouseButtonEventArgs e)
        {
            MenuGrid grid = (sender as MenuGrid)!;
            Grid parent;
            if (grid.Name != "Profile") 
                parent = (Grid)VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(grid));
            else
                parent = (Grid)VisualTreeHelper.GetParent(grid);
            foreach (var child in parent.Children)
            {
                if (child is MenuGrid)
                {
                    MenuGrid menu = (child as MenuGrid)!;
                    menu.IsSelected = false;
                }
                else
                {
                    StackPanel panel = (child as StackPanel)!;
                    if (panel == null)
                        continue;
                    foreach (MenuGrid menu in panel.Children)
                    {
                        menu.IsSelected = false;
                    }
                }
            }
            grid.IsSelected = true;

            var typeOfGridContent = MainFrame.Content?.GetType();
            if (grid.Name == "Friends" && typeOfGridContent != typeof(FriendsMenuPage))
                MainFrame.Navigate(new FriendsMenuPage());
            if (grid.Name == "Profile" && typeOfGridContent != typeof(ProfilePage))
                MainFrame.Navigate(new ProfilePage());
        }

        private async void SetProfileInformation()
        {
            await applicationData.LoadedTask;
            LoginLabel.Content = applicationData.UserProfile.Login;
            if (applicationData.UserProfile.ImageByte is null)
                return;
            var image = App.GetImageFromByteArray(applicationData.UserProfile.ImageByte);
            ProfileImage.Source = image;
            SetProfileImage(image);

        }

        private async void Logout(object sender, MouseButtonEventArgs e)
        {
            var response = await httpClient.PostAsync(App.HttpsStr + "/logout", null);
            httpClient.DefaultRequestHeaders.Authorization = null;
            var window = new InitialWindow();
            window.Show();
            this.Close();
        }

        internal void SetProfileImage(BitmapImage image) =>
            ProfileImage.Source = image;

        internal void OpenUserProfile(object sender, MouseButtonEventArgs e, ProfileInformation? profile = null)
        {
            ProfileInformation userProfile;
            if (profile is null)
            {
                Grid grid = sender as Grid;
                userProfile = grid.DataContext as ProfileInformation;
            }
            else
                userProfile = profile;
            if (ProfileFrame.Visibility == Visibility.Visible)
            {
                OtherUserProfilePage content = ProfileFrame.Content as OtherUserProfilePage;
                if (content.Profile.Login == userProfile.Login)
                    return;
            }
            ProfileFrame.Navigate(new OtherUserProfilePage(userProfile));
            ProfileFrame.Visibility = Visibility.Visible;
        }

        private void ProfileFrame_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ProfileFrameColumn.Width = ProfileFrame.Visibility == Visibility.Visible ? new GridLength(0.4, GridUnitType.Star) : new GridLength(0);
        }
    }
}
