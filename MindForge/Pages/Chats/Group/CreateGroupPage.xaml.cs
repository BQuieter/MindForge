using Microsoft.Win32;
using MindForge;
using MindForgeClasses;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MindForgeClient.Pages.Chats.Group
{
    public partial class CreateGroupPage : Page
    {
        private HttpClient httpClient;
        private MainWindow currentWindow;
        private ApplicationData applicationData;
        private byte[] currentImage = null;
        private List<ProfileInformation> selectedFriends = new();
        private event EventHandler cancelEvent;
        private event EventHandler createEvent;
        public CreateGroupPage(EventHandler CancelEvent, EventHandler CreatelEvent)
        {
            InitializeComponent();
            cancelEvent = CancelEvent;
            createEvent = CreatelEvent;
            httpClient = HttpClientSingleton.httpClient;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            currentWindow = Window.GetWindow(this) as MainWindow;
            applicationData = currentWindow.applicationData;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) =>
            App.WatermarkHelper(sender, e);

        private void FilterFriends(object sender, TextChangedEventArgs e)
        {
            App.WatermarkHelper(sender, e);
            TextBox textBox = (TextBox)sender;
            if (textBox.Text.Length == 0)
                FriendsListBox.ItemsSource = applicationData.UsersFriends;
            else
            {
                var filteredCollection = applicationData.UsersFriends.Where(u => Regex.IsMatch(u.Login.ToLower(), $"^.*{textBox.Text.ToLower()}.*$"));
                var filteredChats = new ObservableCollection<ProfileInformation>(filteredCollection);
                FriendsListBox.ItemsSource = filteredChats;
            }
        }

        private void Friend_Loaded(object sender, RoutedEventArgs e)
        {
            MenuGrid menuGrid = sender as MenuGrid;
            var friend = menuGrid.DataContext as ProfileInformation;
            if (selectedFriends.Contains(friend))
                menuGrid.IsSelected = true;
        }

        private void ListBox_Loaded(object sender, RoutedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            listBox.ItemsSource = applicationData.UsersFriends;
        }
        private void ProfileImage_MouseEnter(object sender, MouseEventArgs e) =>
            ((Image)sender).Cursor = Cursors.Hand;

        private void ProfileImage_MouseLeave(object sender, MouseEventArgs e) =>
            ((Image)sender).Cursor = Cursors.Arrow;
            
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

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            Image image = sender as Image;
            ProfileInformation chatInform = image.DataContext as ProfileInformation;
            image.Source = App.GetImageFromByteArray(chatInform.ImageByte);
        }

        private void ChangeImage(object sender, MouseButtonEventArgs e)
        {
            Image image = (sender as Image)!;
            OpenFileDialog photoDialog = new OpenFileDialog();
            photoDialog.Filter = "Image files (*.png;*.jpg;*jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*";
            if (photoDialog.ShowDialog() == false)
                return;
            byte[] imageByte = ProfilePage.CompressImageAndGetBytes(photoDialog.FileName);
            currentImage = imageByte;
            GroupImage.Source = App.GetImageFromByteArray(currentImage);
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

        private void Cancel_Click(object sender, RoutedEventArgs e) =>
            cancelEvent?.Invoke(this, EventArgs.Empty);

        private async void Create_Click(object sender, RoutedEventArgs e)
        {
            GroupChatInformation information = new() { ImageByte = currentImage, Name = GroupNameBox.Text.Length > 0 ? GroupNameBox.Text : applicationData.UserProfile.Login, Members = new ObservableCollection<ProfileInformation>(selectedFriends) };
            information.Members.Add(applicationData.UserProfile);
            var response = await httpClient.PostAsJsonAsync<GroupChatInformation>(App.HttpsStr + "/groupchats/create", information);
            if (!response.IsSuccessStatusCode)
                return;
            int id = await response.Content.ReadFromJsonAsync<int>();
            information.ChatId = id;
            applicationData.GroupChatsInformation.Add(information);
            applicationData.GroupChats.Add(information.ChatId, new());
            createEvent?.Invoke(this, EventArgs.Empty);
            cancelEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
