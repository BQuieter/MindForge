using System.Text;
using System.Text.RegularExpressions;
using BCrypt.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Net.Mail;

namespace MindForge
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        private readonly string regexLogin = @"^[а-яА-Яa-zA-Z0-9-_@.]*$";
        private readonly string regexPassword = @"^[a-zA-Z0-9-_!@#$&.,]*$";

        private Mutex mutex;
        private string mutexName = "MindForge";
        public LoadingWindow()
        {
            //Перенеси в статик метод чтоль
            mutex = new Mutex(false, mutexName, out bool createdNew);

            if (!createdNew)
            {
                WindowHelper.MaximizeWindow("MindForge");
                Application.Current.Shutdown();
                return;
            }
            InitializeComponent();
            this.BorderThickness = SystemParametersFix.WindowResizeBorderThickness;
            ///////////////////////// Когда будет сервак, переделай
            /*Dispatcher.InvokeAsync(async () => 
            {
                await Task.Delay(1500);
                LoadingGrid.Visibility = Visibility.Collapsed;
                SignInGrid.Visibility = Visibility.Visible;
            });*/
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e) =>
            this.Close();

        private void MinimizeButton_Click(object sender, RoutedEventArgs e) =>
                this.WindowState = WindowState.Minimized;

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }
        private void MindForge_Closed(object sender, EventArgs e) =>
            mutex.ReleaseMutex();

        private void LoginBox_TextChanged(object sender, TextChangedEventArgs e) =>
            ChangeWatermarkVisible(sender, LoginBlock);
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e) =>
            ChangeWatermarkVisible(sender, PasswordBlock);
        private void RegistrationLoginBox_TextChanged(object sender, TextChangedEventArgs e) =>
            ChangeWatermarkVisible(sender, RegistrationLoginBlock);
        private void RegistrationEmailBox_TextChanged(object sender, TextChangedEventArgs e) =>
            ChangeWatermarkVisible(sender, RegistrationEmailBlock);
        private void RegistrationPasswordBox_TextChanged(object sender, TextChangedEventArgs e) =>
            ChangeWatermarkVisible(sender, RegistrationPasswordBlock);
        private void ChangeWatermarkVisible(object sender, TextBlock textBlock)
        {
            bool isEmpty = false;
            if (sender is TextBox)
            {
                TextBox textBox = (TextBox)sender;
                isEmpty = textBox.Text.Length == 0;
            }
            else if (sender is PasswordBox)
            {
                PasswordBox passwordBox = (PasswordBox)sender;
                isEmpty = passwordBox.Password.Length == 0;
            }
            textBlock.Visibility = isEmpty ? Visibility.Visible : Visibility.Collapsed;
        }
        private void EnterKeyFocus(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                MainGrid.Focus();
        }
        private void LoseFocus(object sender, MouseButtonEventArgs e) =>
            MainGrid.Focus();

        private void DisableContextMenu(object sender, MouseButtonEventArgs e) =>
            e.Handled = true;

        private void GoToRegisterButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SignInGrid.Visibility = Visibility.Collapsed;
            RegistrationGrid.Visibility = Visibility.Visible;
        }

        private void GoToSignInButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RegistrationGrid.Visibility = Visibility.Collapsed;
            SignInGrid.Visibility = Visibility.Visible;
        }

        private void ResetValues(object sender, DependencyPropertyChangedEventArgs e)
        {
            Grid grid = (Grid)sender;
            foreach(var a in grid.Children)
            {
                if (a is TextBox)
                {
                    TextBox textBox = (TextBox)a;
                    textBox.Text = "";
                }
                else if (a is PasswordBox)
                {
                    PasswordBox passwordBox = (PasswordBox)a;
                    passwordBox.Password = "";
                }
                else if (a is TextBlock)
                {
                    TextBlock textBlock = (TextBlock)a;
                    if (Regex.IsMatch(textBlock.Name, "^.*Warn$"))
                        textBlock.Text = "";
                }
            }
        }

        private void SingInButton_Click(object sender, RoutedEventArgs e)
        {//Сначала парол в Bcrypt потом запрос
            string password = PasswordBox.Password;
            LoginBox.Text = BCrypt.Net.BCrypt.HashPassword(password);
        }
        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWarn.Text = CheckLogin();
            EmailWarn.Text = CheckEmail();
            PasswordWarn.Text = CheckPassword();
            if (LoginWarn.Text != "" || PasswordWarn.Text != "" || EmailWarn.Text != "")
                return;
        }

        private string CheckLogin()
        {
            string login = RegistrationLoginBox.Text;
            if (login.Length < 3)
                return "Логин должен содержать минимум 3 символа";
            if (!Regex.IsMatch(login, regexLogin))
                return "Допустимые символы: а-я, А-Я, a-z, A-Z, 0-9, -_@.";
            return "";
        }

        private string CheckPassword()
        {
            string password = RegistrationPasswordBox.Text;
            if (password.Length < 6)
                return "Пароль должен содержать минимум 6 символов";
            if (!Regex.IsMatch(password, regexPassword))
                return "Допустимые символы: a-z, A-Z, 0-9, -_!@#$&amp;.,";
            return "";
        }

        private string CheckEmail() 
        {
            string email = RegistrationEmailBox.Text;
            if (!MailAddress.TryCreate(email, out var address))
                return "Недопустимый Email";
            return "";
        }
    }
}