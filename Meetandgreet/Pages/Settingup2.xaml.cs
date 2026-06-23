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
        private Preferences currentUser;

        public Settingup2(Preferences usr)
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
                currentUser.ProfilePic = fileDialog.FileName;

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
            if (currentUser == null)
            {
                currentUser = new Preferences();
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
            Gender? preferGender = (await ApiService.GetGenderListIdAsync()).Find(x => x.Id == selectedPrefGenderId);

            Preferences currPreference = new Preferences() { AgeMax = currentUser.AgeMax, AgeMin = currentUser.AgeMin, DistanceMax = currentUser.DistanceMax, PreferredGender= preferGender, Age= currentUser.Age,
             Bio=currentUser.Bio, City=currentUser.City, CreatedAt=currentUser.CreatedAt, DateOfBirth=currentUser.DateOfBirth,
             Email=currentUser.Email, Gender=currentUser.Gender, Password=currentUser.Password, ProfilePic=currentUser.ProfilePic,
             Username=currentUser.Username};
            await ApiService.InsertPreferencesAsync(currPreference);

            // Set your preferences properties normally


            currentUser.PreferredGender = preferGender;

            // --- PASTE THE FIX RIGHT HERE BEFORE SENDING ---
            // 3. To pass API validation without looping, clear the circular object back-reference right before sending
            //currentUser.Preferences.User =  currentUser;
;

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
