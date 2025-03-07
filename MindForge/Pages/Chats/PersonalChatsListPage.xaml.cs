using MindForge;
using MindForgeClasses;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MindForgeClient.Pages.Chats
{
    public partial class PersonalChatsListPage : Page
    {
        private MainWindow currentWindow;
        private ApplicationData applicationData;
        private string lastSelected = null;
        private Dictionary<string, MenuGrid> grids = new();
        private PersonalChatInformation openChat;
        public PersonalChatsListPage()
        {
            InitializeComponent();
        }
        public PersonalChatsListPage(PersonalChatInformation chatInform)
        {
            InitializeComponent();
            openChat = chatInform;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            currentWindow = Window.GetWindow(this) as MainWindow;
            applicationData = currentWindow.applicationData;
            ChatsListBox.ItemsSource = applicationData.PersonalChatsInformation;
            currentWindow.personalChatNotificationService.PersonalChatCreated += PersonalChatCreate;
            if (applicationData.PersonalChatsInformation.Count != 0)
            {
                NoChatsGrid.Visibility = Visibility.Collapsed;
                ChatsGrid.Visibility = Visibility.Visible;
            }
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            currentWindow.personalChatNotificationService.PersonalChatCreated -= PersonalChatCreate;
        }

        private void PersonalChatCreate(object sender, PersonalChatInformation chat)
        {
            Dispatcher.Invoke(() =>
            {
                NoChatsGrid.Visibility= Visibility.Collapsed;
                ChatsGrid.Visibility= Visibility.Visible;
            });
        }

        private void MenuClickHelper(object sender, MouseButtonEventArgs e)
        {
            MenuGrid grid = (sender as MenuGrid)!;
            PersonalChatInformation context = grid.DataContext as PersonalChatInformation;
           if (lastSelected is not null && grids.ContainsKey(lastSelected))
                grids[lastSelected].IsSelected = false;
            grid.IsSelected = true;
            OpenChat(context);
            lastSelected = context.Login;
        }

        private void CreateChat(object sender, RoutedEventArgs e) =>
            currentWindow.GoToFriendPage();

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            Image image = sender as Image;
            PersonalChatInformation chatInform = image.DataContext as PersonalChatInformation;
            image.Source = App.GetImageFromByteArray(chatInform.ImageByte);
        }

        private void Chat_Loaded(object sender, RoutedEventArgs e)
        {
            MenuGrid grid = sender as MenuGrid;
            PersonalChatInformation context = grid.DataContext as PersonalChatInformation;
            if(!grids.ContainsKey(context.Login))
                grids.Add(context.Login, grid);
            if(openChat is not null && context.Login == openChat.Login)
            {
                grid.IsSelected = true;
                OpenChat(context);
                lastSelected = context.Login;
            }
            if (lastSelected is not null && context.Login == lastSelected)
                grid.IsSelected = true;
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
                var filteredCollection = applicationData.PersonalChatsInformation.Where(u => Regex.IsMatch(u.Login.ToLower(), $"^.*{textBox.Text.ToLower()}.*$"));
                var filteredChats = new ObservableCollection<PersonalChatInformation>(filteredCollection);
                ChatsListBox.ItemsSource = filteredChats;
            }
        }

        private void OpenChat(PersonalChatInformation profileInformation)
        {
            if (lastSelected == profileInformation.Login)
                return;
            MainFrame.Navigate(new ChatPage(profileInformation));
        }
    }
}
