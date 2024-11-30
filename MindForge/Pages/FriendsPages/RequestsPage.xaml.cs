using System;
using System.Collections.Generic;
using System.Linq;
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

namespace MindForgeClient.Pages.FriendsPages
{
    /// <summary>
    /// Логика взаимодействия для RequestsPage.xaml
    /// </summary>
    public partial class RequestsPage : Page
    {
        public RequestsPage()
        {
            InitializeComponent();
            MainFrame.Navigate(new IncomingRequestsPage());
        }

        private void MenuClick(object sender, MouseButtonEventArgs e)
        {
            MenuGrid grid = (sender as MenuGrid)!;
            StackPanel parent = (StackPanel)VisualTreeHelper.GetParent(grid);
            foreach (var child in parent.Children)
            {
                if (child is MenuGrid menuGrid)
                    menuGrid.IsSelected = false;
            }
            grid.IsSelected = true;

            var typeOfGridContent = MainFrame.Content?.GetType();
            if (grid.Name == "IncomingRequests" && typeOfGridContent != typeof(IncomingRequestsPage))
                MainFrame.Navigate(new IncomingRequestsPage());
            if (grid.Name == "OutgoingRequests" && typeOfGridContent != typeof(OutgoingRequestsPage))
                MainFrame.Navigate(new OutgoingRequestsPage());
           
        }
    }
}
