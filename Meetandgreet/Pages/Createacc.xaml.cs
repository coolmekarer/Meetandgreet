using interfaceapi;
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


            // 2. Validation
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

                // Now you can save 'age' and 'birthday' to your User object
                User. = birthday;
                User.CurrentUser.Age = age;
                NavigationService.Navigate(new Settingup(newUser));

            //try
            //{
            //    Apiinter api = new Apiinter();
            //    int result = await api.InsertAUser(newUser);

            //    if (result == 1)
            //    {
            //        MessageBox.Show("Welcome! Your account has been created.");
            //        NavigationService.Navigate(new Settingup()); 
            //    }
            //    else
            //    {
            //        MessageBox.Show("Signup failed. That email might already be registered.");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Error: {ex.Message}");
            //}
        }
    }
}
