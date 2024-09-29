using System.Text;
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

namespace MindForge
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        private Mutex mutex;
        private string mutexName = "MyApplicationMutex";
        public LoadingWindow()
        {
            mutex = new Mutex(false, mutexName, out bool createdNew);

            if (!createdNew)
            {
                WindowHelper.MaximizeWindow("MindForge");
                Application.Current.Shutdown();
                return;
            }
            InitializeComponent();
            this.BorderThickness = SystemParametersFix.WindowResizeBorderThickness;
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
        //
        private void MindForge_Closed(object sender, EventArgs e) =>
            mutex.ReleaseMutex();

        private void LoginBox_GotFocus(object sender, RoutedEventArgs e) =>
            TextBoxHelp((TextBox)sender, "Логин или email", "", new SolidColorBrush(Color.FromRgb(160, 160, 160)));

        private void LoginBox_LostFocus(object sender, RoutedEventArgs e) =>   
            TextBoxHelp((TextBox)sender, "", "Логин или email", new SolidColorBrush(Color.FromRgb(112, 112, 112)));

        private void TextBoxHelp(TextBox textBox, string expectedText, string changeText, SolidColorBrush brush)
        {
            if (textBox.Text == expectedText)
            {
                textBox.Text = changeText;
                textBox.Foreground = brush;
            }
        }
    }
}