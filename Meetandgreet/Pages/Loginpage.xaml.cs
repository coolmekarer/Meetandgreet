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
using ModelDates;

namespace Meetandgreet.Pages
{
    /// <summary>
    /// Interaction logic for Loginpage.xaml
    /// </summary>
    public partial class Loginpage : Page
    {
        public Loginpage()
        {
            InitializeComponent();
        }

       

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

      

        private async void CheckLogIn_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailInput.Text;
            string password = PasswordInput.Password;

            // 1. Check if inputs are empty
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.");
                return; // Stop here if empty
            }

            // 2. Get the list from your API
            // Note: Make sure GetAllUser() exists in ApiService.cs and returns a List<User>
            var userList = await ApiService.GetAllUser();

            // 3. Find the user (Fixed the lambda syntax here)
            var user = userList.FirstOrDefault(x => x.Email == email && x.Password == password);

            // 4. Check if the user was found
            if (user == null)
            {
                MessageBox.Show("Invalid username or password.");
            }
            else
            {

                NavigationService.Navigate(new Homepage(user));
            }
        }
    }
    }
    

