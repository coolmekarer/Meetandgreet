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
        private Preferences _newUser;

        // FIXED: The constructor now takes the 'User' object directly from Createacc page
        public Settingup(Preferences userFromSignup)
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
                MessageBox.Show("Minimum age preference cannot be greater than maximum age.");
                return;
            }

            // --- ADD THIS CRITICAL FIX HERE ---
            // If the preferences package doesn't exist yet, build a new empty container for it!
            if (_newUser == null)
            {
                _newUser = new Preferences();
            }

            // Now C# can safely store these numbers without throwing a Null exception!
            _newUser.AgeMin = MinAge;
            _newUser.AgeMax = MaxAge;
            _newUser.DistanceMax = maxDistanceKm; // Sync this if needed!

            // Navigate to step 2 safely
            NavigationService.Navigate(new Settingup2(_newUser));
        }
    }
}