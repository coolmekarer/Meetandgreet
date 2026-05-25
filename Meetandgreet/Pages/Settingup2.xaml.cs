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
using ModelDates; // Accesses your User models

namespace Meetandgreet.Pages
{
    /// <summary>
    /// Interaction logic for Settingup2.xaml
    /// </summary>
    public partial class Settingup2 : Page
    {
        private User currentUser;

        public Settingup2(User usr)
        {
            InitializeComponent();
            currentUser = usr;
        }

        private void UploadPic_Click(object sender, RoutedEventArgs e)
        {
            // Open a standard Windows file Explorer selection box
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";

            if (fileDialog.ShowDialog() == true)
            {
                // 1. Save the local file directory path straight into your user object payload
                currentUser.Profilepic = fileDialog.FileName;

                // 2. Dynamically change the XAML profile circle to preview the chosen image instantly!
                BitmapImage bitmap = new BitmapImage(new Uri(fileDialog.FileName));
                ProfileImageBrush.ImageSource = bitmap;
            }
        }
        // Changed to 'async void' so we can await our background API call
        private async void Finish_Click(object sender, RoutedEventArgs e)
        {
            // 1. Gather text from the 'About Me' TextBox
            currentUser.Bio = aboutMe.Text;

            // 2. Clear out or instantiate the Preferences object if it's null
            if (currentUser.Preferences == null)
            {
                currentUser.Preferences = new Preferences();
            }

            // Map the radio buttons to your gender preference settings
            int selectedPrefGenderId = 3; // Default to Both
            string selectedPrefGenderName = "Both";

            if (PrefMalesBtn.IsChecked == true)
            {
                selectedPrefGenderId = 2;
                selectedPrefGenderName = "Male";
            }
            else if (PrefFemalesBtn.IsChecked == true)
            {
                selectedPrefGenderId = 1;
                selectedPrefGenderName = "Female";
            }

            // Set your preferences properties normally
            currentUser.Preferences.PreferredGender = new Gender { Id = selectedPrefGenderId, Name = selectedPrefGenderName };

            // --- PASTE THE FIX RIGHT HERE BEFORE SENDING ---
            // 3. To pass API validation without looping, clear the circular object back-reference right before sending
            currentUser.Preferences.User = null;

            // 4. Fire the request safely across the Dev Tunnel!
            bool isSuccess = await ApiService.RegisterUserAsync(currentUser);

            if (isSuccess)
            {
                MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                // You can uncomment your navigation code here to go to the main feed/dashboard!
                 NavigationService.Navigate(new Homepage(currentUser));
            }
            else
            {
                MessageBox.Show("We couldn't finalize your registration. Please verify your internet link or try an alternate email.", "Registration Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
