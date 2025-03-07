using MindForge;
using MindForgeClasses;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MindForgeClient.Pages
{
    /// <summary>
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        private Regex passwordHasLowerChar  = new(@"[a-zа-я]+");
        private Regex passwordHasUpperChar = new(@"[A-ZА-Я]+");
        private Regex passwordHasNumber = new(@"[0-9]+");
        private Regex hasIncorrectSymbols = new("""["' ]+""");
        private Regex hasSpace = new(" ");
        private HttpClient httpClient;
        public RegistrationPage()
        {
            InitializeComponent();
            httpClient = HttpClientSingleton.httpClient!;
        }

        private void GoToSignInPage(object sender, MouseButtonEventArgs e)
        {
            Window window = Window.GetWindow(this);
            Frame frame = (Frame)window.FindName("InitialFrame");
            if (frame != null)
                frame.Navigate(new SignInPage());
        }

        private void LoginBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            WatermarkHelper(sender, e);
            TextBox textBox = (sender as TextBox)!;
            string text = textBox.Text;
            string warn = "";

            if (text.Length < 3 && text.Length != 0)
                warn = "Логин должен содержать минимум 3 символа";
            if (hasIncorrectSymbols.IsMatch(text))
                warn = "Логин не должен содержать символы \" \' и пробел";

            LoginWarn.Text = warn;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            WatermarkHelper(sender, e);
            PasswordBox passwordBox = (sender as PasswordBox)!;
            string password = passwordBox.Password;
            string warn = "";
            if (password.Length != 0)
            {
                if (password.Length < 8)
                    warn = "Пароль должен содержать минимум 8 символов";
                if (!passwordHasNumber.IsMatch(password))
                    warn = "Пароль должен содержать цифру";
                if (!passwordHasUpperChar.IsMatch(password))
                    warn = "Пароль должен содержать букву в верхнем регистре";
                if (!passwordHasLowerChar.IsMatch(password))
                    warn = "Пароль должен содержать букву в нижнем регистре";
                if (hasIncorrectSymbols.IsMatch(password))
                    warn = "Логин не должен содержать символы \" \' и пробел";
            }
            ConfirmPasswordBox_PasswordChanged(ConfirmPasswordBox as object, new());

            PasswordWarn.Text = warn;
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            WatermarkHelper(sender, e);
            PasswordBox confirmPasswordBox = (sender as PasswordBox)!;
            string password = confirmPasswordBox.Password;
            string warn = "";

            if (password != PasswordBox.Password && password.Length != 0)
                warn = "Пароли не совпадают";

            ConfirmPasswordWarn.Text = warn;
        }
        private void WatermarkHelper(object sender, TextChangedEventArgs e) =>
            App.WatermarkHelper(sender, e);
        private void WatermarkHelper(object sender, RoutedEventArgs e) =>
            App.WatermarkHelper(sender, e);

        private async void Registration(object sender, RoutedEventArgs e)
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
            if (PasswordBox.Password != string.Empty && ConfirmPasswordBox.Password == string.Empty)
            {
                ConfirmPasswordWarn.Text = "Подтвердите пароль";
                reject = true;
            }
            if (reject || (LoginWarn.Text != string.Empty && PasswordWarn.Text != string.Empty && ConfirmPasswordWarn.Text != string.Empty))
                return;
            InitialWindow.ShowLoadingGif(LoadingGif, RegistrationButton);
            UserLoginInformation information = new(LoginBox.Text, BCrypt.Net.BCrypt.HashPassword(PasswordBox.Password));
            var response = await httpClient.PostAsJsonAsync<UserLoginInformation>(App.HttpsStr + "/registration", information);
            if (response.IsSuccessStatusCode)
            {
                await InitialWindow.GetJwtToken(response);
                InitialWindow window = (InitialWindow)Window.GetWindow(this);
                window.GoToMainWindow(window);
            }
            else
                await LoginExists(response);
            InitialWindow.HideLoadingGif(LoadingGif, RegistrationButton);
        }

        private async Task LoginExists(HttpResponseMessage response)
        {
            var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            LoginWarn.Text = error!.Message;
        }
    }
}
