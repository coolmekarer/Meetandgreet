using ModelDates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for Createacc.xaml
    /// </summary>
    public partial class Createacc : Page
    {
        public Createacc()
        {
            InitializeComponent();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private async void SignUp_Click(object sender, RoutedEventArgs e)
        {
            string fullName = NameInput.Text.Trim();
            string email = EmailInput.Text.Trim();
            string password = PassInput.Password;

            // Validation
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please fill out all fields to join the garden.");
                return;
            }

            int selectedGenderId = 3;
            if (MaleBtn.IsChecked == true) selectedGenderId = 2;
            else if (FemaleBtn.IsChecked == true) selectedGenderId = 1;

            User newUser = new User
            {
                Username = fullName,
                Email = email,
                Password = password,
                Gender = new Gender { Id = selectedGenderId, Name = "Default" }
            };

            DateTime? selectedDate = BirthdatePicker.SelectedDate;

            if (selectedDate.HasValue)
            {
                DateTime birthday = selectedDate.Value;
                int age = DateTime.Today.Year - birthday.Year;

                // Adjust for leap years/months
                if (birthday > DateTime.Today.AddYears(-age)) age--;

                if (age < 18)
                {
                    MessageBox.Show("You must be at least 18 to join the garden.");
                    return;
                }

                // FIXED: Assign directly to your newUser object instance
                newUser.DateOfBirth = birthday; // Verify if your model uses 'Birthdate' or 'Birthday'
                newUser.Age = age;

                // FIXED: Capitalized to match your 'Settingup2.xaml' layout styling if needed
                NavigationService.Navigate(new Settingup(newUser));
            }
        }
    }
}

