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

        // Changed to 'async void' so we can await our background API call
        private async void Finish_Click(object sender, RoutedEventArgs e)
        {
            // 1. Capture the biography text
            currentUser.Bio = aboutMe.Text.Trim();

            // 2. Determine gender preference ID based on selected RadioButton
            // Assuming your backend setup uses a system like: 1 = Female, 2 = Male, 3 = Both/All
            int selectedPrefGenderId = 3;
            if (PrefMalesBtn.IsChecked == true)
            {
                selectedPrefGenderId = 2;
            }
            else if (PrefFemalesBtn.IsChecked == true)
            {
                selectedPrefGenderId = 1;
            }

            // 3. Assign the preference object to your user profile
            // Adjust this line to match your exact DB Preference object structure if needed
            currentUser.PrefferedGender = new Gender { Id = selectedPrefGenderId, Name = "Preference" };

            // 4. Send the completely populated profile payload to the database via API
            try
            {
                // This utilizes your static registration method inside ApiService.cs!
                bool isSuccess = await ApiService.RegisterUserAsync(currentUser);

                if (isSuccess)
                {
                    MessageBox.Show("Welcome to the garden! Your profile is set up.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Navigate to your main application board
                    NavigationService.Navigate(new Homepage(currentUser));
                }
                else
                {
                    MessageBox.Show("We couldn't finalize your registration. Please verify your internet link or try an alternate email.", "Registration Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database Connection Error: {ex.Message}", "API Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
