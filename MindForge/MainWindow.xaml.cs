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
        private readonly string regexLogin = @"^[а-яА-Яa-zA-Z0-9-_@.]*$";
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
            ///////////////////////// Когда будет сервак, переделай
            Dispatcher.InvokeAsync(async () => 
            {
                await Task.Delay(1500);
                LoadingGrid.Visibility = Visibility.Collapsed;
                SignInGrid.Visibility = Visibility.Visible;
            });
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
            }
        }
    }
}