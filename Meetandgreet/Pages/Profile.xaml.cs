using ModelDates;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Input;
using Microsoft.Win32;

namespace Meetandgreet.Pages
{
    public partial class Profile : Page
    {
        private User currentUser;
        private int _userId;

        public Profile(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadProfileData();
        }

        private async void LoadProfileData()
        {
            try
            {
                var user = await ApiService.GetUserByIdAsync(_userId);

                if (user != null)
                {
                    currentUser = user;

                    UsernameTxt.Text = user.Username;
                    AgeTxt.Text = user.Age.ToString();
                    BioTxt.Text = user.Bio;

                    if (!string.IsNullOrEmpty(user.ProfilePic) && System.IO.File.Exists(user.ProfilePic))
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(user.ProfilePic);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        ProfilePhoto.Source = bitmap;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading profile: " + ex.Message);
            }

            await LoadGallery();
        }

        private async Task LoadGallery()
        {
            var photos = await ApiService.GetPhotosByUserIdAsync(_userId);
            GalleryGrid.ItemsSource = photos;
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

    

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            currentUser = null;
            while (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }
            NavigationService.Navigate(new Loginpage());
        }

        private async void DeleteAccBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete your account? This cannot be undone.",
                                         "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                bool success = await ApiService.DeleteUserAsync(_userId);
                if (success)
                {
                    MessageBox.Show("Account deleted successfully.");
                    NavigationService.Navigate(new Uri("Pages/Loginpage.xaml", UriKind.Relative));
                }
                else
                {
                    MessageBox.Show("Failed to delete account. Please try again.");
                }
            }
        }

        private void EditProfileBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Editprofile(currentUser));
        }

        // --- Gallery Management ---

        private void EditGalleryBtn_Click(object sender, RoutedEventArgs e)
        {
            GalleryEditor.Visibility = (GalleryEditor.Visibility == Visibility.Visible)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private async void DeletePic_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var photo = btn.DataContext as Photos;

            if (photo != null)
            {
                bool success = await ApiService.DeletePhotoAsync(photo.Id);
                if (success) await LoadGallery();
            }
        }
        private async void AddPhoto_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg";

            if (openFileDialog.ShowDialog() == true)
            {
                string newPath = openFileDialog.FileName;

                // Ensure you are using the full 'currentUser' object
                // so that ph.User.Id can be retrieved by PhotosDB
                // In Profile.xaml.cs, temporarily add this for debugging
                System.Diagnostics.Debug.WriteLine($"DEBUG: Sending Photo - URL: {newPath}, UserID: {currentUser?.Id}");

                if (currentUser == null || currentUser.Id == 0)
                {
                    MessageBox.Show("Error: User data is missing or incomplete. Cannot upload photo.");
                    return;
                }

                var newPhoto = new Photos
                {
                    User = currentUser,
                    Url = newPath
                };

                // Now you are sending just the ID and the URL, which is exactly what the DB needs
                bool success = await ApiService.UploadPhotoAsync(currentUser.Id, newPath);
                if (success)
                {
                   await LoadGallery();
                }
                else
                {
                    MessageBox.Show("Upload failed. Ensure the API is receiving the full User object.");
                }
            }
        }

        private async void ChangePic_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var oldPhoto = btn.DataContext as Photos; // Get the old photo data

            if (oldPhoto == null) return;

            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg";

            if (openFileDialog.ShowDialog() == true)
            {
                string newPath = openFileDialog.FileName;

                // 1. Delete the old photo from the database first
                bool deleteSuccess = await ApiService.DeletePhotoAsync(oldPhoto.Id);

                if (deleteSuccess)
                {
                    // 2. Upload the new photo
                    var newPhoto = new Photos { User = currentUser, Url = newPath };
                    // Pass the ID and the path as two separate arguments
                    bool uploadSuccess = await ApiService.UploadPhotoAsync(currentUser.Id, newPath);

                    if (uploadSuccess)
                    {
                        // 3. Refresh the UI to show the new picture
                       // await LoadGallery();
                    }
                    else
                    {
                        MessageBox.Show("Photo deleted, but failed to upload new photo.");
                    }
                }
                else
                {
                    MessageBox.Show("Failed to remove old photo. Please try again.");
                }
            }
        }

        // --- Hover Effects Helpers ---

        private void ShowButtons_MouseEnter(object sender, MouseEventArgs e)
        {
            var border = sender as Border;
            var stack = FindVisualChild<StackPanel>(border);
            if (stack != null) stack.Visibility = Visibility.Visible;
        }

        private void HideButtons_MouseLeave(object sender, MouseEventArgs e)
        {
            var border = sender as Border;
            var stack = FindVisualChild<StackPanel>(border);
            if (stack != null) stack.Visibility = Visibility.Collapsed;
        }

        private T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = System.Windows.Media.VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T) return (T)child;
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null) return childOfChild;
                }
            }
            return null;
        }
    }
}