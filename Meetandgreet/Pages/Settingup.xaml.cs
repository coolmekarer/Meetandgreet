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
using ModelDates; // Keeps track of your User model

namespace Meetandgreet.Pages
{
    /// <summary>
    /// Interaction logic for Settingup.xaml
    /// </summary>
    public partial class Settingup : Page
    {
        // This will temporarily store the user object passed from Createacc
        private User _newUser;

        // FIXED: The constructor now takes the 'User' object directly from Createacc page
        public Settingup(User userFromSignup)
        {
            InitializeComponent();
            _newUser = userFromSignup;
        }

        private void NextStep_Click(object sender, RoutedEventArgs e)
        {
            int MinAge = (int)MinAgeSlider.Value;
            int MaxAge = (int)MaxAgeSlider.Value;
            int maxDistanceKm = (int)DistSlider.Value;

            if (MinAge > MaxAge)
            {
                MessageBox.Show("Minimum age preference cannot be older than maximum age preference.");
                return;
            }

            // Assigning directly to your separate preferences class inside the user!
            _newUser.Preferences.MinAge = MinAge;
            _newUser.Preferences.MaxAge = MaxAge;
            _newUser.Preferences.MaxDistanceKM = maxDistanceKm; // Match your exact preference property names

            NavigationService.Navigate(new Settingup2(_newUser));
        }
    }
}