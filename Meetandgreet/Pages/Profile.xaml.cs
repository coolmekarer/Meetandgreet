using ModelDates; // Ensure this matches your project's namespace
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Meetandgreet.Pages
{
    public partial class Profile : Page
    {
        // We initialize this to store the ID of the user we are looking at
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

                    // ADD THIS PART:
                    if (!string.IsNullOrEmpty(user.ProfilePic) && System.IO.File.Exists(user.ProfilePic))
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(user.ProfilePic);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad; // Force reload
                        bitmap.EndInit();

                        // Replace 'ProfileImage' with the x:Name of your Image control in XAML
                        ProfilePhoto.Source = bitmap;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading profile: " + ex.Message);
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }
        private void ColorPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ColorPicker.SelectedItem is ComboBoxItem selectedItem)
            {
                string hexColor = selectedItem.Tag.ToString();
                // Use the BrushConverter to convert the hex string to an actual Brush
                this.Background = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFrom(hexColor);
            }
        }
        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            // 1. Clear user memory
            currentUser = null;

            // 2. Remove all previous pages from the history
            while (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }

            // 3. Navigate to login
            NavigationService.Navigate(new Loginpage());
        }

        private async void DeleteAccBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete your account? This cannot be undone.",
                                         "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                // 1. Call your API to delete the user
                bool success = await ApiService.DeleteUserAsync(_userId);

                if (success)
                {
                    MessageBox.Show("Account deleted successfully.");
                    NavigationService.Navigate(new Uri("Pages/Login.xaml", UriKind.Relative));
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
    }
}