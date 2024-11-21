using MindForge;
using MindForgeClient.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
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
        private static HttpClient httpClient;
        public MainWindow()
        {
            InitializeComponent();
            this.BorderThickness = SystemParametersFix.WindowResizeBorderThickness;
            httpClient = HttpClientSingleton.httpClient!;
            GetLogin();
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
            if (grid.Name != "ProfileGrid") 
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
                    foreach (MenuGrid menu in panel.Children)
                    {
                        menu.IsSelected = false;
                    }
                }
            }
            grid.IsSelected = true;
            MainFrame.Navigate(new ProfilePage());
        }

        private async void GetLogin()
        {
            string login = await httpClient.GetStringAsync("https://localhost:7236/profile");
            LoginLabel.Content = login;
        }
    }
}
