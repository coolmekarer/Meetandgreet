using ModelDates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            var userList = await ApiService.GetAllUser();
            var user = userList.FirstOrDefault(x => x.Email == email);

            if (user == null)
            {
                MessageBox.Show("Email not found.");
                return;
            }

            // 1. Check if the password typed IS the Manager Password
            bool isManager = await ApiService.VerifyManagerPasswordAsync(user.Id, password);
            Debug.WriteLine($"Manager Check Result for user {user.Id}: {isManager}"); // <--- ADD THIS

            if (isManager)
            {
                user.IsManager = true;
                MessageBox.Show("Logged in as Manager.");
                NavigationService.Navigate(new ManagerHomepage(user));
            }
            // 2. Check if the password typed IS the User Password
            else if (user.Password == password)
            {
                user.IsManager = false;
                NavigationService.Navigate(new Homepage(user));
            }
            else
            {
                MessageBox.Show("Incorrect password.");
            }
        }
    }
    }
    

