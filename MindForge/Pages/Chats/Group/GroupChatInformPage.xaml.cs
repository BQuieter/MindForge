using MindForge;
using MindForgeClasses;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MindForgeClient.Pages.Chats.Group
{
    public partial class GroupChatInformPage : Page
    {
        internal string GroupName {  get; private set; }
        private MainWindow currentWindow;
        private HttpClient httpClient;
        private ApplicationData applicationData;
        private GroupChatInformation chatInformation;
        private List<ProfileInformation> selectedFriends = new();

        public GroupChatInformPage(GroupChatInformation chat)
        {
            InitializeComponent();
            httpClient = HttpClientSingleton.httpClient;
            chatInformation = chat;
            GroupName = chatInformation.Name;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            currentWindow = Window.GetWindow(this) as MainWindow;
            applicationData = currentWindow.applicationData;
            MembersListBox.ItemsSource = chatInformation.Members;
            GroupNameTextBlock.Text = chatInformation.Name;
            if (chatInformation.ImageByte is not null)
                GroupImage.Source = App.GetImageFromByteArray(chatInformation.ImageByte);
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            Image image = sender as Image;
            ProfileInformation userInform = image.DataContext as ProfileInformation;
            if (userInform is not null && userInform.ImageByte is not null)
                image.Source = App.GetImageFromByteArray(userInform.ImageByte);
        }

        private void Close_Click(object sender, RoutedEventArgs e) =>
            currentWindow.CloseProfileFrame();

        private void IsOwnerLoaded(object sender, RoutedEventArgs e)
        {
            Image image = sender as Image;
            var context = image.DataContext as ProfileInformation;
            if (chatInformation.Creator == context.Login)
                image.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Star.png")); 
        }

        private void DeleteButtonLoaded(object sender, RoutedEventArgs e)
        {
            Button image = sender as Button;
            var context = image.DataContext as ProfileInformation;
            if (chatInformation.Creator == applicationData.UserProfile.Login || applicationData.UserProfile.Login == context.Login)
                image.Visibility = Visibility.Visible;
        }

        private void CheckProfile(object sender, MouseButtonEventArgs e)
        {
            MenuGrid grid = sender as MenuGrid;
            var context = grid.DataContext as ProfileInformation;
            currentWindow.CloseProfileFrame();
            currentWindow.OpenProfileFrame(new object(),context);
        }

        private void ScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = sender as ScrollViewer;

            double newVerticalOffset = scrollViewer.VerticalOffset - e.Delta;
            newVerticalOffset = Math.Max(0, newVerticalOffset);
            newVerticalOffset = Math.Min(scrollViewer.ScrollableHeight, newVerticalOffset);
            scrollViewer.ScrollToVerticalOffset(newVerticalOffset);
            e.Handled = true;
        }

        private void FilterFriends(object sender, TextChangedEventArgs e)
        {
            App.WatermarkHelper(sender, e);
            TextBox textBox = (TextBox)sender;
            if (textBox.Text.Length == 0)
                FriendsListBox.ItemsSource = applicationData.UsersFriends.ExceptBy(chatInformation.Members.Select(c => c.Login), c => c.Login);
            else
            {
                var filteredCollection = applicationData.UsersFriends.ExceptBy(chatInformation.Members.Select(c => c.Login), c => c.Login).Where(u => Regex.IsMatch(u.Login.ToLower(), $"^.*{textBox.Text.ToLower()}.*$"));
                var filteredChats = new ObservableCollection<ProfileInformation>(filteredCollection);
                FriendsListBox.ItemsSource = filteredChats;
            }
        }

        private void FriendsListBox_Loaded(object sender, RoutedEventArgs e)
        {
            FriendsListBox.ItemsSource = applicationData.UsersFriends.ExceptBy(chatInformation.Members.Select(c => c.Login), c => c.Login);
        }

        private void MenuGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MenuGrid menuGrid = sender as MenuGrid;
            var friend = menuGrid.DataContext as ProfileInformation;
            menuGrid.IsSelected = !menuGrid.IsSelected;
            if (menuGrid.IsSelected)
                selectedFriends.Add(friend);
            else
                selectedFriends.Remove(friend);
        }

        private void AddMembers_Click(object sender, RoutedEventArgs e)
        {
            AddGridButton.Visibility = Visibility.Collapsed;
            AddGrid.Visibility = Visibility.Visible;
            FriendsListBox.ItemsSource = applicationData.UsersFriends.ExceptBy(chatInformation.Members.Select(c => c.Login), c => c.Login);

        }

        private void MenuGrid_Loaded(object sender, RoutedEventArgs e)
        {
            MenuGrid menuGrid = sender as MenuGrid;
            var friend = menuGrid.DataContext as ProfileInformation;
            if (selectedFriends.Contains(friend))
                menuGrid.IsSelected = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) =>
            CloseEdit();

        private void CloseEdit()
        {
            AddGrid.Visibility = Visibility.Collapsed;
            AddGridButton.Visibility = Visibility.Visible;
            selectedFriends.Clear();
        }
        private async void DeleteMember_Click(object sender, RoutedEventArgs e)
        {
            Button image = sender as Button;
            var context = image.DataContext as ProfileInformation;
            var response = await httpClient.PostAsJsonAsync<GroupChatInformation>(App.HttpsStr + $"/groupchats/delete/{context.Login}", chatInformation);
            if (!response.IsSuccessStatusCode)
                return;
            chatInformation.Members.Remove(context);
            if (context.Login == applicationData.UserProfile.Login)
                currentWindow.personalChatNotificationService.FireYouDeletedEvent(this, chatInformation.ChatId);

        }

        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            var response = await httpClient.PostAsJsonAsync<List<ProfileInformation>>(App.HttpsStr + $"/groupchats/add/{chatInformation.ChatId}", selectedFriends);
            if (!response.IsSuccessStatusCode)
                return;
            foreach (var member in selectedFriends)
                chatInformation.Members.Add(member);
            MembersListBox.ItemsSource = chatInformation.Members;
            foreach (var profile in selectedFriends.ToList())
                if (selectedFriends.Select(u => u.Login).ToList().Contains(profile.Login))
                    selectedFriends.Remove(profile);
            FriendsListBox.ItemsSource = applicationData.UsersFriends.ExceptBy(chatInformation.Members.Select(c => c.Login), c => c.Login);
            CloseEdit();
        }

    }
}
