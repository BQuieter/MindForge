using MindForgeClasses;
using MindForgeClient;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MindForge
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Mutex mutex;
        public static readonly string HttpsStr = "https://localhost:7236";
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            bool createdNew;
            string mutexName = "MindForge";
            mutex = new Mutex(true, mutexName, out createdNew);
            if (!createdNew)
            {
                WindowHelper.MaximizeWindow("MindForge");
                this.Shutdown();
            }
            HttpClientSingleton.Set();
        }

        private async void Application_Exit(object sender, ExitEventArgs e)
        {
            mutex.ReleaseMutex();
            var response = await HttpClientSingleton.httpClient.PostAsync(App.HttpsStr +"/logout", null);
        }
        public static void CloseWindow(Window window) =>
            window.Close();
        public static void MinimizeWindow(Window window) =>
                window.WindowState = WindowState.Minimized;

        public static void MaximizeWindow(Window window)
        {
            if (window.WindowState == WindowState.Maximized)
                window.WindowState = WindowState.Normal;
            else
                window.WindowState = WindowState.Maximized;
        }
        public static void WatermarkHelper(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (sender as TextBox)!;
            var grid = VisualTreeHelper.GetParent(textBox) as Grid;
            grid!.Children[1].Visibility = textBox.Text == string.Empty ? Visibility.Visible : Visibility.Collapsed;
        }

        public static void WatermarkHelper(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = (sender as PasswordBox)!;
            var grid = VisualTreeHelper.GetParent(passwordBox) as Grid;
            grid!.Children[1].Visibility = passwordBox.Password == string.Empty ? Visibility.Visible : Visibility.Collapsed;
        }

        public static BitmapImage GetImageFromByteArray(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = ms;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                return image;
            }
        }
    }
}
