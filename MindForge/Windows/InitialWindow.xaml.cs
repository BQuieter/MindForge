using System.Windows;
using System.Windows.Controls;
using MindForgeClient;
using MindForgeClasses;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using MindForgeClient.Pages;
using System.Windows.Threading;

namespace MindForge
{
    public partial class InitialWindow : Window
    {
        private static HttpClient httpClient;
        public InitialWindow()
        {
            InitializeComponent();

            httpClient = HttpClientSingleton.httpClient!;
            this.BorderThickness = SystemParametersFix.WindowResizeBorderThickness;
            Dispatcher.InvokeAsync(async () =>
            {
                await Check();
                LoadingPanel.Visibility = Visibility.Collapsed;
                InitialFrame.Navigate(new SignInPage());
                async Task<bool> Check()
                {
                    if (!await CheckServerConnection())
                    {
                        await Task.Delay(500);
                        await Check();
                    }
                    return true;
                }
            });

        }
        private async Task<bool> CheckServerConnection()
        {
            var response = await httpClient.GetAsync(App.HttpsStr + "/");
            return response.IsSuccessStatusCode;
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e) =>
            App.CloseWindow(this);
        private void MinimizeButton_Click(object sender, RoutedEventArgs e) =>
            App.MinimizeWindow(this);
        private void MaximizeButton_Click(object sender, RoutedEventArgs e) =>
            App.MaximizeWindow(this);

        public static async Task GetJwtToken(HttpResponseMessage response)
        {
            var jwtTokenResponse = await response.Content.ReadFromJsonAsync<JwtTokenResponse>();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(jwtTokenResponse!.TokenType, jwtTokenResponse.Token);
        }

        public static void ShowLoadingGif(Image gif, Button button)
        {
            Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                button.Visibility = Visibility.Collapsed;
                gif.Visibility = Visibility.Visible;
            });
        }

        public static void HideLoadingGif(Image gif, Button button)
        {
            Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                gif.Visibility = Visibility.Collapsed;
                button.Visibility = Visibility.Visible;
            });
        }
        public void GoToMainWindow(Window window)
        {
            App app = Application.Current as App;
            MainWindow mainWindow = new MainWindow(app.container.GetInstance<IFriendNotificationService>(), app.container.GetInstance<IPersonalChatNotificationService>(), app.container.GetInstance<ICallsService>());
            mainWindow.Width = window.Width;
            mainWindow.Height = window.Height;
            mainWindow.Left = window.Left;
            mainWindow.Top = window.Top;
            mainWindow.Show();
            window.Close();
        }
    }
}