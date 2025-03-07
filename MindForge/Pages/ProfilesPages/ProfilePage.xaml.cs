using Microsoft.Win32;
using MindForge;
using MindForgeClasses;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace MindForgeClient.Pages
{
    public partial class ProfilePage : Page
    {
        private bool isEdit = false;
        private byte[] currentImage = null;
        private HttpClient httpClient;
        private MainWindow currentWindow;
        private ApplicationData applicationData;
        private Dictionary<ProfessionInformation, bool> professionsDifferences = new();
        public ProfilePage()
        {
            InitializeComponent();
            httpClient = HttpClientSingleton.httpClient!;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            currentWindow = (Window.GetWindow(this) as MainWindow)!;
            applicationData = currentWindow.applicationData;
            ProfessionListBox.ItemsSource = applicationData.UserProfessions;
            SetInitialInformation();
        }

        private void ProfileImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!isEdit)
                return;
            Image image = (sender as Image)!;
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
            currentWindow.SetProfileImage(image);
        }

        private void Description_TextChanged(object sender, TextChangedEventArgs e) =>
            App.WatermarkHelper(sender, e);

        private void ChangeProfessions()
        {
            foreach (var profession in professionsDifferences)
            {
                if (profession.Value)
                    applicationData.UserProfessions.Remove(profession.Key);
                else
                    applicationData.UserProfessions.Add(profession.Key);
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Description.Text = applicationData.UserProfile.Description;
            if (applicationData.UserProfile.ImageByte is not null)
                SetImage(applicationData.UserProfile.ImageByte);
            else
                ProfileImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Profile.png"));

            ChangeProfessions();
            StopEdit();
        }

        private async void Edit_Click(object sender, RoutedEventArgs e)
        {
            isEdit = true;
            currentImage = applicationData.UserProfile.ImageByte;
            Description.IsReadOnly = false;
            EditButton.Visibility = Visibility.Collapsed;
            SaveButton.Visibility = Visibility.Visible;
            CancelButton.Visibility = Visibility.Visible;
            if (applicationData.UserProfessions.Count != applicationData.AllProfessions.Count)
                AddProfessionGrid.Visibility = Visibility.Visible;
            SetVisibleListBoxItemButton(true, await GetGrids());
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            var response1 = await httpClient.PutAsJsonAsync<ProfileInformation>(App.HttpsStr + "/profile", new ProfileInformation { Description = Description.Text, ImageByte = currentImage });
            var response2 = await httpClient.PutAsJsonAsync<List<ProfessionInformation>>(App.HttpsStr + $"/professions", applicationData.UserProfessions.ToList());
            if (!response1.IsSuccessStatusCode || !response2.IsSuccessStatusCode)
                return;
            applicationData.UserProfile.Description = Description.Text;
            StopEdit();
        }

        private async void StopEdit()
        {
            isEdit = false;
            Description.IsReadOnly = true;
            professionsDifferences.Clear();
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
        private void SetInitialInformation()
        {
            LoginTextBlock.Text = applicationData.UserProfile.Login;
            Description.Text = applicationData.UserProfile.Description;
            if (applicationData.UserProfile.ImageByte is null)
                return;
            SetImage(applicationData.UserProfile.ImageByte);
        }

        private void ProfileImage_MouseEnter(object sender, MouseEventArgs e)
        {
            if (isEdit)
                ((Image)sender).Cursor = Cursors.Hand;
        }

        private void ProfileImage_MouseLeave(object sender, MouseEventArgs e) =>
            ((Image)sender).Cursor = Cursors.Arrow;

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
            var profession = button.DataContext as ProfessionInformation;
            applicationData.UserProfessions.Remove(profession);
            if (professionsDifferences.ContainsKey(profession))
                professionsDifferences.Remove(profession);
            else
                professionsDifferences.Add(profession, false);
            RecalculateComboBoxItems();
            if (applicationData.UserProfessions.Count != applicationData.AllProfessions.Count && ProfessionsComboBox.Visibility == Visibility.Collapsed)
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
            ProfessionInformation selectedItem = (ProfessionsComboBox.SelectedItem as ProfessionInformation)!;
            applicationData.UserProfessions.Add(selectedItem);
            if (professionsDifferences.ContainsKey(selectedItem))
                professionsDifferences.Remove(selectedItem);
            else
                professionsDifferences.Add(selectedItem, true);

            RecalculateComboBoxItems();
            if (applicationData.UserProfessions.Count != applicationData.AllProfessions.Count)
                AddProfessionGrid.Visibility = Visibility.Visible;
            professionComboBox.SelectedItem = null;
            SetVisibleListBoxItemButton(true, await GetGrids());
        }

        private void RecalculateComboBoxItems()
        {
            List<ProfessionInformation> exitingProfessionsCopy = new List<ProfessionInformation>(applicationData.AllProfessions);

            foreach (var a in applicationData.UserProfessions)
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
                string format = Path.GetExtension(filePath);
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

        private void Page_Unloaded(object sender, RoutedEventArgs e) =>
            ChangeProfessions();
    }
}
