using ModelDates;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Meetandgreet.Pages
{
    public partial class Homepage : Page
    {
        private User currentUser;
        private bool _isMenuExpanded = false;

        private List<User> _discoveryQueue = new List<User>();
        private int _queueIndex = 0;

        // Photo Gallery Management State
        private List<string> _currentProfileGallery = new List<string>();
        private int _currentPhotoIndex = 0;
        private bool _isBioExpanded = false;

        public Homepage(User usr)
        {
            InitializeComponent();
            currentUser = usr;

            // Fire off the real matching engine background worker thread
            LoadRealDiscoveryFeed();
        }

        private async void LoadRealDiscoveryFeed()
        {
            // Fetch preferences first
            Preferences userPrefs = await ApiService.GetPreferencesByUserIdAsync(currentUser.Id);

            // Pass BOTH arguments
            _discoveryQueue = await ApiService.GetDiscoveryFeedAsync(currentUser, userPrefs);

            _queueIndex = 0;

            // Display the first available card
            ShowNextDiscoveryProfile();
        }
        private void ShowNextDiscoveryProfile()
        {
            if (_discoveryQueue != null && _queueIndex < _discoveryQueue.Count)
            {
                User nextMatch = _discoveryQueue[_queueIndex];

                // FIX: Changed LoadRealDiscoveryFeed to LoadUserProfileOnFeed
                LoadUserProfileOnFeed(nextMatch, null);
            }
            else
            {
                // Out of people! Show an empty state
                FeedUsernameTxt.Text = "No more matches found!";
                FeedAgeTxt.Text = "";
                FeedBioTxt.Text = "Try widening your age preferences in your settings.";
                CurrentDisplayedImage.Source = new BitmapImage(new Uri("/Images/defaultprofile.png", UriKind.RelativeOrAbsolute));
            }
        }

        // Make sure this method is defined directly below inside your class!
        // Paste this complete block over your existing two methods inside Homepage.xaml.cs
        private void LoadUserProfileOnFeed(User displayedUser, List<Photos> userPhotos)
        {
            _currentProfileGallery.Clear();
            _currentPhotoIndex = 0;

            // Safety check: Make sure we handle relative or missing image strings safely
            if (!string.IsNullOrEmpty(displayedUser.Profilepic) && displayedUser.Profilepic.StartsWith("/Images"))
            {
                _currentProfileGallery.Add(displayedUser.Profilepic);
            }
            else
            {
                // Use a clean relative fallback path format
                _currentProfileGallery.Add("pack://application:,,,/Images/defaultprofile.png");
            }

            if (userPhotos != null)
            {
                foreach (var photo in userPhotos)
                {
                    _currentProfileGallery.Add(photo.Url);
                }
            }

            // Assigning the text values BEFORE updating the image so they are guaranteed to display!
            FeedUsernameTxt.Text = !string.IsNullOrEmpty(displayedUser.Username) ? displayedUser.Username : "Unknown User";
            FeedAgeTxt.Text = displayedUser.Age > 0 ? displayedUser.Age.ToString() : "--";
            FeedBioTxt.Text = !string.IsNullOrEmpty(displayedUser.Bio) ? displayedUser.Bio : "No bio provided.";

            FeedBioTxt.MaxHeight = 40;
            _isBioExpanded = false;

            UpdateFeedImage();
        }

        private void UpdateFeedImage()
        {
            if (_currentProfileGallery.Count > 0)
            {
                try
                {
                    string targetUri = _currentProfileGallery[_currentPhotoIndex];

                    // FIX: Standard forward-slash separator for the WPF pack URI resource loader
                    if (targetUri.StartsWith("/Images/"))
                    {
                        targetUri = $"pack://application:,,,{targetUri}";
                    }

                    CurrentDisplayedImage.Source = new BitmapImage(new Uri(targetUri, UriKind.RelativeOrAbsolute));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Image Rendering Error: {ex.Message}");
                    CurrentDisplayedImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/defaultprofile.png", UriKind.RelativeOrAbsolute));
                }
            }
        }

        private void NextPic_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPhotoIndex < _currentProfileGallery.Count - 1)
            {
                _currentPhotoIndex++;
                UpdateFeedImage();
            }
        }

        private void PrevPic_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPhotoIndex > 0)
            {
                _currentPhotoIndex--;
                UpdateFeedImage();
            }
        }

        private void ToggleBioExpand_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_isBioExpanded)
            {
                FeedBioTxt.MaxHeight = 40;
            }
            else
            {
                FeedBioTxt.MaxHeight = double.PositiveInfinity;
            }
            _isBioExpanded = !_isBioExpanded;
        }

        private async void LikeUser_Click(object sender, RoutedEventArgs e)
        {
            // 1. Get the current target user from the queue
            if (_discoveryQueue != null && _queueIndex < _discoveryQueue.Count)
            {
                User targetUser = _discoveryQueue[_queueIndex];

                // 2. Call the new API endpoint
                bool isMatch = await ApiService.LikeUserAsync(currentUser.Id, targetUser.Id);

                // 3. Show notification if a match is formed
                if (isMatch)
                {
                    MessageBox.Show($"It's a match with {targetUser.Username}! You can now start texting.",
                                    "New Match Found!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

            // 4. Move to the next profile
            _queueIndex++;
            ShowNextDiscoveryProfile();
        }

        private void PassUser_Click(object sender, RoutedEventArgs e)
        {
            _queueIndex++;
            ShowNextDiscoveryProfile();
        }

        private void ToggleMenu_Click(object sender, RoutedEventArgs e)
        {
            if (!_isMenuExpanded)
            {
                SidebarColumn.Width = new GridLength(200);
                MenuTitleText.Visibility = Visibility.Visible;
                ProfileText.Visibility = Visibility.Visible;
                ChatsText.Visibility = Visibility.Visible;
                _isMenuExpanded = true;
            }
            else
            {
                SidebarColumn.Width = new GridLength(60);
                MenuTitleText.Visibility = Visibility.Collapsed;
                ProfileText.Visibility = Visibility.Collapsed;
                ChatsText.Visibility = Visibility.Collapsed;
                _isMenuExpanded = false;
            }
        }

        private void ProfileNav_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ChatsNav_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}