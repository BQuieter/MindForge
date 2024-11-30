using MindForge;
using MindForgeClasses;
using MindForgeClient.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MindForgeClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal ApplicationData applicationData;
        private static HttpClient httpClient;
        public MainWindow()
        {
            InitializeComponent();
            this.BorderThickness = SystemParametersFix.WindowResizeBorderThickness;
            httpClient = HttpClientSingleton.httpClient!;
            applicationData = new();
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
            Window window = new InitialWindow();
            window.Show();
            this.Close();
        }

        internal void SetProfileImage(BitmapImage image) =>
            ProfileImage.Source = image;
    }
}
