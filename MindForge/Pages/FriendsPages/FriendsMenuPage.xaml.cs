using MindForge;
using MindForgeClasses;
using MindForgeClient.Pages.FriendsPages;
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
using static System.Collections.Specialized.BitVector32;

namespace MindForgeClient.Pages
{
    /// <summary>
    /// Логика взаимодействия для FriendsMenuPage.xaml
    /// </summary>
    public partial class FriendsMenuPage : Page
    {
        private MainWindow currentWindow;
        public FriendsMenuPage()
        {
            InitializeComponent();
            MainFrame.Navigate(new AllFriendsPage());
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            currentWindow = Window.GetWindow(this) as MainWindow;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            currentWindow.CloseProfileFrame();
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
            if (grid.Name == "AddFriends" && typeOfGridContent != typeof(AddFriendsPage))
                MainFrame.Navigate(new AddFriendsPage());
            if (grid.Name == "AllFriends" && typeOfGridContent != typeof(AllFriendsPage))
                MainFrame.Navigate(new AllFriendsPage());
            if (grid.Name == "RequestFriend" && typeOfGridContent != typeof(RequestsPage))
                MainFrame.Navigate(new RequestsPage());
        }

        internal async static Task<HttpResponseMessage> MakeRelationshipAction(RelationshipAction relationshipAction,string target)
        {
            RelationshipRequest relationshipRequest = new RelationshipRequest { RelationshipAction = relationshipAction};
            var relationshipResponse = await HttpClientSingleton.httpClient.PostAsJsonAsync<RelationshipRequest>(App.HttpsStr + $"/relationship/{target}", relationshipRequest);
            return relationshipResponse;
        }

        internal static void Image_Loaded(object sender, RoutedEventArgs e)
        {
            var image = (Image)sender;
            var profile = image.DataContext as ProfileInformation;
            if (profile!.ImageByte is not null)
                image.Source = App.GetImageFromByteArray(profile.ImageByte);
        }
    }
}
