using System.Configuration;
using System.Data;
using System.Windows;

namespace MindForge
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Mutex mutex;
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
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            mutex.ReleaseMutex();
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
    }

}
