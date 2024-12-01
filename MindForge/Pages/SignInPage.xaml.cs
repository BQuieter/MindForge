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
    /// Логика взаимодействия для SignInPage.xaml
    /// </summary>
    public partial class SignInPage : Page
    {
        private HttpClient httpClient;

        public SignInPage()
        {
            InitializeComponent();
            httpClient = HttpClientSingleton.httpClient!;
            LoginBox.Text = "qwerty";
            PasswordBox.Password = "Qwerty12";
            //SignIn(new object(), new RoutedEventArgs());
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Window window = Window.GetWindow(this);
            Frame frame = (Frame)window.FindName("InitialFrame");
            if (frame != null)
                frame.Navigate(new RegistrationPage());
        }
        private void LoginBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            WatermarkHelper(sender, e);
            LoginWarn.Text = string.Empty;
            PasswordWarn.Text = string.Empty;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            WatermarkHelper(sender, e);
            PasswordWarn.Text = string.Empty;
        }
        private void WatermarkHelper(object sender, TextChangedEventArgs e) =>
             App.WatermarkHelper(sender, e);
        private void WatermarkHelper(object sender, RoutedEventArgs e) =>
            App.WatermarkHelper(sender, e);


        private async void SignIn(object sender, RoutedEventArgs e)
        {
            bool reject = false;
            if (LoginBox.Text == string.Empty)
            {
                LoginWarn.Text = "Введите логин";
                reject = true;
            }
            if (PasswordBox.Password == string.Empty)
            {
                PasswordWarn.Text = "Введите пароль";
                reject = true;
            }
            if (reject)
                return;

            UserLoginInformation information = new(LoginBox.Text, PasswordBox.Password);
            InitialWindow.ShowLoadingGif(LoadingGif, SignInButton);
            var response = await httpClient.PostAsJsonAsync<UserLoginInformation>(App.HttpsStr + "/login", information);
            if (response.IsSuccessStatusCode)
            {
                await InitialWindow.GetJwtToken(response);
                InitialWindow window = (InitialWindow)Window.GetWindow(this);
                window.GoToMainWindow(window);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                PasswordWarn.Text = "Неверный пароль";
            else
                await WriteWarn(response);
            InitialWindow.HideLoadingGif(LoadingGif, SignInButton);

        }
        private async Task WriteWarn(HttpResponseMessage response)
        {
            var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            LoginWarn.Text = error!.Message;
        }
    }
}
