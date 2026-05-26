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
    /// Interaction logic for Matcheslist.xaml
    /// </summary>
    public partial class Matcheslist : Page
    {
        private User _currentUser;

        public Matcheslist(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            LoadMatches();
        }

        private async void LoadMatches()
        {
            // Fetch matches from API
            var matches = await ApiService.GetMatchesForUserAsync(_currentUser.Id);
            MatchesListBox.ItemsSource = matches;
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void MatchesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MatchesListBox.SelectedItem is Matches selectedMatch)
            {
                // Navigate to the Chat window passing the selected match
                NavigationService.Navigate(new Chatwindow(selectedMatch, _currentUser));
            }
        } 
    }
}
