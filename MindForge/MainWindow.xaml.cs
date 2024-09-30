using System.Text;
using System.Text.RegularExpressions;
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

namespace MindForge
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        private readonly string regexLogin = @"^[a-zA-Z0-9-_@.]*$";
        private readonly string regexPassword = @"^[a-zA-Z0-9-_!@#$&.,]*$";

        private Mutex mutex;
        private string mutexName = "MyApplicationMutex";
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
            /////////////////////////////////////////////////////////
            
            /*Dispatcher.InvokeAsync(async () => 
            {
                await Task.Delay(2000);
                LoadingGrid.Visibility = Visibility.Collapsed;
                AuthorizationGrid.Visibility = Visibility.Visible;
            });*/
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
                this.WindowState = WindowState.Minimized;
        }

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

        private void LoginBox_GotFocus(object sender, RoutedEventArgs e) =>
            TextBoxTextHelp((TextBox)sender, "Логин или email", "", new SolidColorBrush(Color.FromRgb(160, 160, 160)));

        private void LoginBox_LostFocus(object sender, RoutedEventArgs e) =>
            TextBoxTextHelp((TextBox)sender, "", "Логин или email", new SolidColorBrush(Color.FromRgb(112, 112, 112)));
        private void TextBoxTextHelp(TextBox textBox, string expectedText, string changeText, SolidColorBrush brush)
        {
            if (textBox.Text == expectedText)
            {
                textBox.Text = changeText;
                textBox.Foreground = brush;
            }
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e) =>
            TextBoxTextHelp((PasswordBox)sender, "*****", "", new SolidColorBrush(Color.FromRgb(160, 160, 160)));

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e) =>
            TextBoxTextHelp((PasswordBox) sender, "", "*****", new SolidColorBrush(Color.FromRgb(160, 160, 160)));

        private void TextBoxTextHelp(PasswordBox textBox, string expectedText, string changeText, SolidColorBrush brush)
        {
            if (textBox.Password == expectedText)
            {
                textBox.Password = changeText;
                textBox.Foreground = brush;
            }
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

        private void LoginBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string text =  textBox.Text;
            bool isMatch = Regex.IsMatch(textBox.Text, regexLogin);
            if (!isMatch && text.Length > 1 && text != "Логин или email")
            {
                textBox.Text = text.Substring(0, text.Length - 1);
                textBox.SelectionStart = text.Length-1;
            }
            else if (!isMatch && (text.Length == 1 || text.Length == 0))
                textBox.Text = "";
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {

            PasswordBox passwordBox = (PasswordBox)sender;
            string text = passwordBox.Password;
            bool isMatch = Regex.IsMatch(passwordBox.Password, regexPassword);

            if (!isMatch && text.Length > 1 && text != "*****")
            {
                passwordBox.Password = text.Substring(0, text.Length - 1);
                SetSelection(passwordBox, text.Length - 1, 0);
            }
            else if (!isMatch && (text.Length == 1 || text.Length == 0))
                passwordBox.Password = "";
        }
        private void SetSelection(PasswordBox passwordBox, int start, int length)
        {
            passwordBox.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(passwordBox, new object[] { start, length });
        }
    }
}