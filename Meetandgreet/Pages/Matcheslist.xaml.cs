using ModelDates;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Meetandgreet.Pages
{
    public partial class Matcheslist : Page
    {
        private User _currentUser;

        public Matcheslist(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            this.Loaded += Matcheslist_Loaded;
        }

        private void Matcheslist_Loaded(object sender, RoutedEventArgs e)
        {
            LoadMatches();
        }

        private async void LoadMatches()
        {
            var matches = await ApiService.GetMatchesForUserAsync(_currentUser.Id);

            // This creates the 'DisplayName' for the UI
            var displayList = matches.Select(m => new {
                OriginalMatch = m,
                DisplayName = m.GetOtherUser(_currentUser.Id)?.Username ?? "Unknown"
            }).ToList();

            MatchesListBox.ItemsSource = displayList;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack) NavigationService.GoBack();
        }

        private void MatchesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MatchesListBox.SelectedItem != null)
            {
                dynamic selectedItem = MatchesListBox.SelectedItem;
                Matches selectedMatch = selectedItem.OriginalMatch;

                NavigationService.Navigate(new Chatwindow(selectedMatch, _currentUser));
                MatchesListBox.SelectedItem = null;
            }
        }
    }
}