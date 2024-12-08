using MindForge;
using MindForgeClasses;
using MindForgeClient.Pages.Chats.Group;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace MindForgeClient.Pages.Chats
{
    /// <summary>
    /// Логика взаимодействия для GroupChatsPage.xaml
    /// </summary>
    public partial class GroupChatsPage : Page
    {
        private MainWindow currentWindow;
        private ApplicationData applicationData;
        private event EventHandler CancelEvent;
        private event EventHandler CreateEvent;
        private Dictionary<string, MenuGrid> grids = new();
        private string lastSelected = null;
        public GroupChatsPage()
        {
            InitializeComponent();
            CancelEvent += CancelCreate;
            CreateEvent += GroupCreate;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            currentWindow = Window.GetWindow(this) as MainWindow;
            applicationData = currentWindow.applicationData;
            ChatsListBox.ItemsSource = applicationData.GroupChatsInformation;
            currentWindow.personalChatNotificationService.GroupChatCreated += GroupCreated;
            if (applicationData.GroupChatsInformation.Count > 0)
            {
                NoChatsGrid.Visibility = Visibility.Collapsed;
                ChatsGrid.Visibility = Visibility.Visible;
            }
        }

        private void CancelCreate(object sender, EventArgs e) =>
            MainFrame.Navigate(null);

        private void GroupCreated(object sender, GroupChatInformation e) =>
           HideWarn();

        private void GroupCreate(object sender, EventArgs e) =>
            HideWarn();
       

        private void GroupCreate_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content?.GetType() != typeof(CreateGroupPage))
                MainFrame.Navigate(new CreateGroupPage(CancelEvent, CreateEvent));

        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            Image image = sender as Image;
            GroupChatInformation information = image.DataContext as GroupChatInformation;
            if (information.ImageByte is not null)
                image.Source = App.GetImageFromByteArray(information.ImageByte);
        }

        private void HideWarn()
        {
            Dispatcher.Invoke(() =>
            {
                NoChatsGrid.Visibility = Visibility.Collapsed;
                ChatsGrid.Visibility = Visibility.Visible;
            });
        }
        private void FilterChats(object sender, TextChangedEventArgs e)
        {
            grids.Clear();
            App.WatermarkHelper(sender, e);
            TextBox textBox = (TextBox)sender;
            if (textBox.Text.Length == 0)
                ChatsListBox.ItemsSource = applicationData.PersonalChatsInformation;
            else
            {
                var filteredCollection = applicationData.GroupChatsInformation.Where(u => Regex.IsMatch(u.Name.ToLower(), $"^.*{textBox.Text.ToLower()}.*$"));
                var filteredChats = new ObservableCollection<GroupChatInformation>(filteredCollection);
                ChatsListBox.ItemsSource = filteredChats;
            }
        }
        private void MenuClickHelper(object sender, MouseButtonEventArgs e)
        {
            MenuGrid grid = (sender as MenuGrid)!;
            GroupChatInformation context = grid.DataContext as GroupChatInformation;
            if (lastSelected is not null && grids.ContainsKey(lastSelected))
                grids[lastSelected].IsSelected = false;
            grid.IsSelected = true;
            OpenChat(context);
            lastSelected = context.Name;
        }

        private void OpenChat(GroupChatInformation profileInformation)
        {
            if (lastSelected == profileInformation.Name)
                return;
            MainFrame.Navigate(new ChatPage(profileInformation));
        }

        private void Chat_Loaded(object sender, RoutedEventArgs e)
        {
            MenuGrid grid = sender as MenuGrid;
            GroupChatInformation context = grid.DataContext as GroupChatInformation;
            if (!grids.ContainsKey(context.Name))
                grids.Add(context.Name, grid);
            if (lastSelected is not null && context.Name == lastSelected)
                grid.IsSelected = true;
        }
    }
}
