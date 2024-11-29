using Microsoft.Win32;
using MindForge;
using MindForgeClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Printing;
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
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace MindForgeClient.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        private bool isEdit = false;
        //Штука чтоб сохранить поля профиля для отката
        private Dictionary<string, object> oldData = new();
        private byte[] currentImage = null;
        private HttpClient httpClient;
        // -----------------------------------
        private Window currentWindow;
        private ProfileInformation profileInformation;
        private List<ProfessionResponse> exitingProfessions;
        private ObservableCollection<ProfessionResponse> userProfessions = new ObservableCollection<ProfessionResponse>();
        private ObservableCollection<ProfessionResponse> oldUserProfessions = new ObservableCollection<ProfessionResponse>();

        public ProfilePage()
        {
            InitializeComponent();
            httpClient = HttpClientSingleton.httpClient!;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            currentWindow = Window.GetWindow(this);
            profileInformation = (ProfileInformation)currentWindow.Resources["Profile"];
            exitingProfessions = (List<ProfessionResponse>)currentWindow.Resources["Professions"];
            SetInitialInformation();
            ProfessionListBox.ItemsSource = userProfessions;
        }

        private void ProfileImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!isEdit)
                return;
            System.Windows.Controls.Image image = (sender as System.Windows.Controls.Image)!;
            OpenFileDialog photoDialog = new OpenFileDialog();
            photoDialog.Filter = "Image files (*.png;*.jpg;*jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*";
            if (photoDialog.ShowDialog() == false)
                return;
            byte[] imageByte = CompressImageAndGetBytes(photoDialog.FileName);
                currentImage = imageByte;
            SetImage(imageByte);

        }

        private void SetImage(byte[] imageByte)
        {
            var image = App.GetImageFromByteArray(imageByte);
            ProfileImage.Source = image;
            MainWindow window = (currentWindow as MainWindow)!;
            window.SetProfileImage(image);
        }

        private void Description_TextChanged(object sender, TextChangedEventArgs e) =>
            App.WatermarkHelper(sender, e);

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Description.Text = oldData["Description"].ToString();
            if (oldData["Image"] is not null)
            {
                currentImage = (byte[])oldData["Image"];
                SetImage(currentImage);
            }
            else
                ProfileImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Profile.png"));
            if (oldUserProfessions.Count > 0)
            {
                userProfessions.Clear();
                userProfessions = new ObservableCollection<ProfessionResponse>(oldUserProfessions.Select(p => p));
            }
            else
                userProfessions.Clear();
            StopEdit();
        }

        private async void Edit_Click(object sender, RoutedEventArgs e)
        {
            isEdit = true;
            Description.IsReadOnly = false;
            EditButton.Visibility = Visibility.Collapsed;
            SaveButton.Visibility = Visibility.Visible;
            CancelButton.Visibility = Visibility.Visible;
            if (userProfessions.Count != exitingProfessions.Count)
                AddProfessionGrid.Visibility = Visibility.Visible;
            SetVisibleListBoxItemButton(true, await GetGrids());
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            var response = await httpClient.PutAsJsonAsync<ProfileInformation>(App.HttpsStr + "/profile", new ProfileInformation { Description = Description.Text, ImageByte = currentImage });
            var responsea = await httpClient.PutAsJsonAsync<List<ProfessionResponse>>(App.HttpsStr + $"/professions/{profileInformation.Login}", userProfessions.ToList());
            if (!response.IsSuccessStatusCode)
                return;
            oldData["Description"] = Description.Text;
            oldData["Image"] = currentImage;
            oldUserProfessions = new ObservableCollection<ProfessionResponse>(userProfessions.Select(p => p));
            StopEdit();
        }

        private async void StopEdit()
        {
            ProfessionListBox.ItemsSource = userProfessions;
            isEdit = false;
            Description.IsReadOnly = true;
            EditButton.Visibility = Visibility.Visible;
            SaveButton.Visibility = Visibility.Collapsed;
            CancelButton.Visibility = Visibility.Collapsed;
            AddProfessionGrid.Visibility = Visibility.Collapsed;
            ProfessionsComboBox.Visibility = Visibility.Collapsed;
            SetVisibleListBoxItemButton(false, await GetGrids());
        }

        private async Task<List<Grid>> GetGrids()
        {
            await Task.Delay(10);
            List<Grid> grids = new List<Grid>();
            foreach (object item in ProfessionListBox.Items)
            {
                ListBoxItem listBoxItem = (ListBoxItem)ProfessionListBox.ItemContainerGenerator.ContainerFromItem(item);
                if (listBoxItem != null)
                {
                    Grid grid = FindVisualChild<Grid>(listBoxItem);
                    if (grid != null)
                    {
                        grids.Add(grid);
                    }
                }
            }
            return grids;
        }
        private async void SetInitialInformation()
        {
            LoginTextBlock.Text = profileInformation.Login;
            Description.Text = profileInformation.Description;
            oldData["Description"] = profileInformation.Description;
            await GetUserProfessions();
            oldUserProfessions = new ObservableCollection<ProfessionResponse>(userProfessions.Select(p => p));

            if (profileInformation.ImageByte is null)
                return;
            oldData["Image"] = profileInformation.ImageByte;
            currentImage = profileInformation.ImageByte;
            SetImage(profileInformation.ImageByte);
        }

        private async Task GetUserProfessions()
        {
            var response = await httpClient.GetAsync(App.HttpsStr + $"/professions/{profileInformation.Login}");
            if (!response.IsSuccessStatusCode)
                return;
            userProfessions = new ObservableCollection<ProfessionResponse>(await response.Content.ReadFromJsonAsync<List<ProfessionResponse>>());
            ProfessionListBox.ItemsSource = userProfessions;
        }

        private void ProfileImage_MouseEnter(object sender, MouseEventArgs e)
        {
            if (isEdit)
                ((System.Windows.Controls.Image)sender).Cursor = Cursors.Hand;
        }

        private void ProfileImage_MouseLeave(object sender, MouseEventArgs e) =>
            ((System.Windows.Controls.Image)sender).Cursor = Cursors.Arrow;

        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child != null && child is T)
                {
                    return (T)child;
                }
                T childItem = FindVisualChild<T>(child);
                if (childItem != null) return childItem;
            }
            return null;
        }
        private void DeleteProfession(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var profession = button.DataContext as ProfessionResponse;
            userProfessions.Remove(userProfessions.FirstOrDefault(p => p.Name == profession.Name));
            RecalculateComboBoxItems();
            if (userProfessions.Count != exitingProfessions.Count && ProfessionsComboBox.Visibility == Visibility.Collapsed)
                AddProfessionGrid.Visibility = Visibility.Visible;
        }

        private void AddProfessionGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Grid grid = (sender as Grid)!;
            grid.Visibility = Visibility.Collapsed;
            ProfessionsComboBox.Visibility = Visibility.Visible;
        }

        private async void ChooseProfession(object sender, SelectionChangedEventArgs e)
        {
            ComboBox professionComboBox = (sender as ComboBox)!;
            if (professionComboBox.SelectedItem is null)
                return;
            professionComboBox.Visibility = Visibility.Collapsed;
            ProfessionResponse selectedItem = (ProfessionsComboBox.SelectedItem as ProfessionResponse)!;
            userProfessions.Add(selectedItem);
            ProfessionListBox.ItemsSource = userProfessions;
            RecalculateComboBoxItems();
            if (userProfessions.Count != exitingProfessions.Count)
                AddProfessionGrid.Visibility = Visibility.Visible;
            professionComboBox.SelectedItem = null;
            SetVisibleListBoxItemButton(true, await GetGrids());
        }

        private void RecalculateComboBoxItems()
        {
            List<ProfessionResponse> exitingProfessionsCopy = new List<ProfessionResponse>(exitingProfessions);

            foreach (var a in userProfessions)
                exitingProfessionsCopy.RemoveAll(p => p.Name == a.Name);

            ProfessionsComboBox.ItemsSource = exitingProfessionsCopy;
        }
        private void ProfessionsComboBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ProfessionsComboBox.Visibility == Visibility.Visible)
                RecalculateComboBoxItems();
        }

        private void SetVisibleListBoxItemButton(bool visible, List<Grid> grids)
        {
            foreach (Grid grid in grids)
            {
                foreach (var element in grid.Children)
                {
                    if (element.GetType() == typeof(Button))
                    {
                        Button button = (Button)element;
                        button.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
                        button.InvalidateVisual();
                    }
                }
            }
        }
        public static byte[] CompressImageAndGetBytes(string filePath, int quality = 85)
        {
            try
            {
                string format = System.IO.Path.GetExtension(filePath);
                using (var image = SixLabors.ImageSharp.Image.Load(filePath))
                {
                    using (var outputStream = new MemoryStream())
                    {
                        
                        image.Mutate(x => x.AutoOrient());
                        image.Mutate(x => x.Resize(new SixLabors.ImageSharp.Size(120, 120)));

                        if (format.ToLower() == ".jpg" || format.ToLower() == ".jpeg")
                        {
                            image.Save(outputStream, new JpegEncoder { Quality = quality });
                        }
                        else if (format.ToLower() == ".png")
                        {
                            image.Save(outputStream, new PngEncoder());
                        }
                        else
                        {
                            throw new NotSupportedException($"Формат {format} не поддерживается.");
                        }

                        return outputStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сжатии изображения: {ex.Message}");
                return null; 
            }
        }
    }
}
