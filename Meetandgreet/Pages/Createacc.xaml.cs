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
    public partial class Createacc : Page
    {
        public Createacc()
        {
            InitializeComponent();
            LoadCities(); // Async loader trigger
        }

        // Fetch city options from your active Dev Tunnel endpoint
        private async void LoadCities()
        {
            try
            {
                // This targets the endpoint listed on your Swagger page
                List<City> citiesList = await ApiService.GetCitiesAsync();

                if (citiesList != null && citiesList.Count > 0)
                {
                    CityDropdown.ItemsSource = citiesList;
                    CityDropdown.SelectedIndex = 0; // Default to first item
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load cities list: " + ex.Message);
            }
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
            City selectedCity = CityDropdown.SelectedItem as City;

            // Form validations
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please fill out all fields to join the garden.");
                return;
            }

            if (selectedCity == null)
            {
                MessageBox.Show("Please select your city location.");
                return;
            }

            int selectedGenderId = 3;
            if (MaleBtn.IsChecked == true) selectedGenderId = 2;
            else if (FemaleBtn.IsChecked == true) selectedGenderId = 1;
            Gender currentGender =(await ApiService.GetGenderListIdAsync()).Find(x=>x.Id==selectedGenderId);
            Preferences newUser = new Preferences
            {
                Username = fullName,
                Email = email,
                Password = password,
                Gender = currentGender,
                City = selectedCity, // ATTACHED VALID LOCATION ENTITY HERE!
                CreatedAt = DateTime.Now
            };

            DateTime? selectedDate = BirthdatePicker.SelectedDate;

            if (selectedDate.HasValue)
            {
                DateTime birthday = selectedDate.Value;
                int age = DateTime.Today.Year - birthday.Year;

                if (birthday > DateTime.Today.AddYears(-age)) age--;

                if (age < 18)
                {
                    MessageBox.Show("You must be at least 18 to join the garden.");
                    return;
                }

                newUser.DateOfBirth = birthday;
                newUser.Age = age;

                // Move forward with the populated user data
                NavigationService.Navigate(new Settingup(newUser));
            }
            else
            {
                MessageBox.Show("Please select a valid birthdate.");
            }
        }
    }
}

