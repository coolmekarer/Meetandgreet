using Microsoft.Win32;
using ModelDates;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Meetandgreet.Pages
{
    public partial class Editprofile : Page
    {
        private User _currentUser;

        // Required constructor for NavigationService
        public Editprofile() { InitializeComponent(); }

        public Editprofile(User user)
        {
            InitializeComponent();
            _currentUser = user;

            BioInput.Text = _currentUser.Bio;

            // Only try to load if the string is a valid path
            if (!string.IsNullOrWhiteSpace(_currentUser.ProfilePic) && System.IO.File.Exists(_currentUser.ProfilePic))
            {
                try
                {
                    ProfileImageBrush.ImageSource = new BitmapImage(new Uri(_currentUser.ProfilePic));
                }
                catch { }
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack) NavigationService.GoBack();
        }

        private void UploadPic_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog { Filter = "Image files (*.png;*.jpg)|*.png;*.jpg" };
            if (fileDialog.ShowDialog() == true)
            {
                _currentUser.ProfilePic = fileDialog.FileName;
                ProfileImageBrush.ImageSource = new BitmapImage(new Uri(fileDialog.FileName));
            }
        }

        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            _currentUser.Bio = BioInput.Text;
            System.Diagnostics.Debug.WriteLine($"DEBUG: Sending Profile Picture Path: {_currentUser.ProfilePic}");

            // Change 'bool' to 'string' to catch the status message
            string result = await ApiService.UpdateUserAsync(_currentUser);

            // Check if the result starts with "OK" (200)
            if (result.Contains("OK"))
            {
                MessageBox.Show("Profile updated successfully!");
                NavigationService.GoBack();
            }
            else
            {
                // Show the actual error message returned by the API
                MessageBox.Show("Update failed: " + result);
            }
        }
    }
}