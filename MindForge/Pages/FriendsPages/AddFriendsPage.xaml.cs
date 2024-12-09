using MindForge;
using MindForgeClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.Serialization.Formatters;
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
    /// Логика взаимодействия для AddFriends.xaml
    /// </summary>
    public partial class AddFriendsPage : Page
    {
        private HttpClient httpClient;
        private MainWindow currentWindow;
        private ProfileInformation currentTarget;
        private ApplicationData applicationData;
        public AddFriendsPage()
        {
            InitializeComponent();
            httpClient = HttpClientSingleton.httpClient!;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            currentWindow = Window.GetWindow(this) as MainWindow;
            applicationData = currentWindow.applicationData;
            currentWindow.friendNotificationService.FriendAdded += FriendAdd;
            currentWindow.friendNotificationService.FriendDeleted += FriendDelete;
            currentWindow.friendNotificationService.FriendRequestReceived += RequestAdd;
            currentWindow.friendNotificationService.FriendRequestDeleted += RequestDelete;
            currentWindow.friendNotificationService.FriendRequestRejected += RequestReject;
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            currentWindow.friendNotificationService.FriendAdded -= FriendAdd;
            currentWindow.friendNotificationService.FriendDeleted -= FriendDelete;
            currentWindow.friendNotificationService.FriendRequestReceived -= RequestAdd;
            currentWindow.friendNotificationService.FriendRequestDeleted -= RequestDelete;
            currentWindow.friendNotificationService.FriendRequestRejected -= RequestReject;
        }

        private void FriendAdd(object sender, ProfileInformation profile)
        {
            if (profile.Login != currentTarget.Login)
                return;
            Dispatcher.Invoke(() =>
            {
                RecalculateVisibilityButtons(Relationship.Friends);
                ActionTextBlock.Text = "Пользователь принял ваш запрос";
                ActionTextBlock.Visibility = Visibility.Visible;
            });
        }
        private void FriendDelete(object sender, ProfileInformation profile)
        {
            if (profile.Login != currentTarget.Login)
                return;
            Dispatcher.Invoke(() =>
            {
                RecalculateVisibilityButtons(Relationship.None);
                ActionTextBlock.Text = "Пользователь удалил вас из друзей";
                ActionTextBlock.Visibility = Visibility.Visible;
            });
        }
        private void RequestAdd(object sender, ProfileInformation profile)
        {
            if (profile.Login != currentTarget.Login)
                return;
            Dispatcher.Invoke(() =>
            {
                RecalculateVisibilityButtons(Relationship.RequestSented);
                ActionTextBlock.Text = "Пользователь отправил запрос";
                ActionTextBlock.Visibility = Visibility.Visible;
            });

        }
        private void RequestDelete(object sender, ProfileInformation profile)
        {
            if (profile.Login != currentTarget.Login)
                return;
            Dispatcher.Invoke(() =>
            {
                RecalculateVisibilityButtons(Relationship.None);
                ActionTextBlock.Text = "Пользователь отозвал запрос";
                ActionTextBlock.Visibility = Visibility.Visible;
            });
        }
        private void RequestReject(object sender, ProfileInformation profile)
        {
            if (profile.Login != currentTarget.Login)
                return;
            Dispatcher.Invoke(() =>
            {
                RecalculateVisibilityButtons(Relationship.None);
                ActionTextBlock.Text = "Пользователь отклонил ваш запрос";
                ActionTextBlock.Visibility = Visibility.Visible;
            });
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) 
        {
            App.WatermarkHelper(sender, e);
            UserWarn.Visibility = Visibility.Collapsed;
            ActionTextBlock.Visibility = Visibility.Collapsed;
        }

        private async void SearchUser(object sender, RoutedEventArgs e)
        {
            if (LoginTextBox.Text == "")
                return;
            if(LoginTextBox.Text == applicationData.UserProfile.Login)
            {
                UserWarn.Text = "Пользователь с данным логином это ты";
                UserWarn.Visibility = Visibility.Visible;
                return;
            }
            if (currentTarget is not null && currentTarget.Login == LoginTextBox.Text)
                return;
            ActionTextBlock.Visibility = Visibility.Collapsed;
            var profileResponse = await httpClient.GetAsync(App.HttpsStr + $"/profile/{LoginTextBox.Text}");
            var relationshipResponse = await httpClient.GetAsync(App.HttpsStr + $"/relationship/{LoginTextBox.Text}");
            if (profileResponse.IsSuccessStatusCode && relationshipResponse.IsSuccessStatusCode)
            {
                var userInform = await profileResponse.Content.ReadFromJsonAsync<ProfileInformation>();
                currentTarget = userInform;
                var relationshipInform = await relationshipResponse.Content.ReadFromJsonAsync<UserRelationshipResponse>();
                UserLogin.Text = currentTarget!.Login;
                UserImage.Source = App.GetImageFromByteArray(userInform.ImageByte);
                RecalculateVisibilityButtons(relationshipInform.Relationship, relationshipInform.IsYouInitiator);
                UserWarn.Visibility = Visibility.Collapsed;
                UserGrid.Visibility = Visibility.Visible;
            }
            else
            {
                var error = await profileResponse.Content.ReadFromJsonAsync<ErrorResponse>();
                UserWarn.Text = error?.Message;
                UserGrid.Visibility = Visibility.Collapsed;
                UserWarn.Visibility = Visibility.Visible;
            }
        }
        private void RecalculateVisibilityButtons(Relationship relationship, bool? isYouInitiator = false) 
        {
            HideButtons();
            switch (relationship)
            {
                case Relationship.None:
                    AddFriendButton.Visibility = Visibility.Visible;
                    break;
                case Relationship.Friends:
                    DeleteFriendButton.Visibility = Visibility.Visible;
                    break;
                case Relationship.RequestSented:
                    if (!(bool)isYouInitiator!)
                    {
                        ApplyRequestButton.Visibility = Visibility.Visible;
                        RejectRequestButton.Visibility = Visibility.Visible;
                    }
                    else
                        DeleteRequestButton.Visibility = Visibility.Visible;
                    break;
            }
        }
        private void HideButtons()
        {
            RejectRequestButton.Visibility = Visibility.Collapsed;
            AddFriendButton.Visibility = Visibility.Collapsed;
            ApplyRequestButton.Visibility = Visibility.Collapsed;
            DeleteFriendButton.Visibility = Visibility.Collapsed;
            DeleteRequestButton.Visibility = Visibility.Collapsed;
        }

        private void RequestAction(object sender, RoutedEventArgs e)
        {
            MakeAction(RelationshipAction.Request, Relationship.RequestSented);
            applicationData.UsersOutgoingRequests.Add(currentTarget);
            ActionTextBlock.Text = "Запрос отправлен";
        }

        private void DeleteFriend(object sender, RoutedEventArgs e)
        {
            MakeAction(RelationshipAction.Delete, Relationship.None);
            applicationData.UsersFriends.Remove(applicationData.UsersFriends.FirstOrDefault(f => f.Login == currentTarget.Login));
            ActionTextBlock.Text = "Друг удалён";
        }
        private void DeleteRequest(object sender, RoutedEventArgs e)
        {
            MakeAction(RelationshipAction.Delete, Relationship.None);
            applicationData.UsersOutgoingRequests.Remove(applicationData.UsersOutgoingRequests.FirstOrDefault(f => f.Login == currentTarget.Login));
            ActionTextBlock.Text = "Запрос отозван";
        }
        private void RejectRequest(object sender, RoutedEventArgs e)
        {
            MakeAction(RelationshipAction.Delete, Relationship.None);
            applicationData.UsersIncomingRequests.Remove(applicationData.UsersIncomingRequests.FirstOrDefault(f => f.Login == currentTarget.Login));
            ActionTextBlock.Text = "Запрос отклонён";
        }

        private void ApplyAction(object sender, RoutedEventArgs e)
        {
            MakeAction(RelationshipAction.Apply, Relationship.Friends);
            applicationData.UsersFriends.Add(currentTarget);
            ActionTextBlock.Text = "Запрос принят";
        }

        private async void MakeAction(RelationshipAction action, Relationship relationship)
        {
            var relationshipResponse = await FriendsMenuPage.MakeRelationshipAction(action, currentTarget.Login);
            if (relationshipResponse.IsSuccessStatusCode)
            {
                RecalculateVisibilityButtons(relationship, true);
                ActionTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                var errorResponse = await relationshipResponse.Content.ReadFromJsonAsync<ErrorResponse>();
                UserWarn.Text = errorResponse.Message;
                UserGrid.Visibility = Visibility.Collapsed;
                UserWarn.Visibility = Visibility.Visible;
                ActionTextBlock.Visibility = Visibility.Collapsed;
            }
        }
        private void OpenProfile(object sender, MouseButtonEventArgs e) =>
            currentWindow.OpenProfileFrame(sender, currentTarget);
        private void GoToChat(object sender, RoutedEventArgs e) =>
            currentWindow.GoToChat(sender,currentTarget);
    }
}
