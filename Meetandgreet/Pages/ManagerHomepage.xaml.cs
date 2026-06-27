using ModelDates;
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

namespace Meetandgreet.Pages
{
    /// <summary>
    /// Interaction logic for ManagerHomepage.xaml
    /// </summary>
    public partial class ManagerHomepage : Page
    {
        public User CurrentManager { get; set; }
        public ManagerHomepage(User user)
        {
            InitializeComponent();
            LoadData();
            CurrentManager = user;
            // Now you can bind CurrentManager to your UI
        }
        private async void LoadData()
        {
            // Fetches all users via your existing API service
            var list = await ApiService.GetAllUser();
            UserListBox.ItemsSource = list;
        }

        private async void Edit_Click(object sender, RoutedEventArgs e)
        {
            var user = (sender as Button).DataContext as User;
            if (user != null)
            {
                // This will open the profile exactly as it is for the user,
                // allowing the manager to edit, add photos, or delete photos.
                NavigationService.Navigate(new Profile(user.Id));
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            var user = (sender as Button).DataContext as User;
            if (user != null)
            {
                var result = MessageBox.Show($"Are you sure you want to delete {user.Username}?",
                                             "Confirm", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    // Call your API delete method (Make sure this exists in ApiService)
                    bool success = await ApiService.DeleteUserAsync(user.Id);
                    if (success)
                    {
                        LoadData(); // Refresh the list
                    }
                }
            }
        }
    }
}
