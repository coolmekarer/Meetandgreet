using Microsoft.Win32;
using ModelDates;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Meetandgreet.Pages
{
    /// <summary>
    /// Interaction logic for Editprofile.xaml
    /// </summary>
    public partial class Editprofile : Page
    {
        private User _currentUser;

        // Constructor receives the user so we know who to update
        public Editprofile(User user)
        {
            InitializeComponent();
            _currentUser = user;

            // 1. Pre-fill the UI with current data
            BioInput.Text = _currentUser.Bio;
            if (!string.IsNullOrEmpty(_currentUser.Profilepic))
            {
                ProfileImageBrush.ImageSource = new BitmapImage(new Uri(_currentUser.Profilepic));
            }
        }

        private void UploadPic_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog { Filter = "Image files (*.png;*.jpg)|*.png;*.jpg" };
            if (fileDialog.ShowDialog() == true)
            {
                _currentUser.Profilepic = fileDialog.FileName; // Stores the local path
                ProfileImageBrush.ImageSource = new BitmapImage(new Uri(fileDialog.FileName));
            }
        }

        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            // 2. Update the user object with new input
            _currentUser.Bio = BioInput.Text;

            // 3. Send to API
            bool success = await ApiService.UpdateUserAsync(_currentUser);

            if (success)
            {
                MessageBox.Show("Profile updated successfully!");
                NavigationService.GoBack();
            }
            else
            {
                MessageBox.Show("Update failed. Please try again.");
            }
        }
    }
    }
