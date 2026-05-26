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
    /// Interaction logic for Chatwindow.xaml
    /// </summary>
    public partial class Chatwindow : Page
    {
        private Matches _currentMatch;
        private User _currentUser; // Add this field!

        // Update your constructor to accept the user
        public Chatwindow(Matches match, User currentUser)
        {
            InitializeComponent();
            _currentMatch = match;
            _currentUser = currentUser; // Store it
            LoadChatHistory();
        }
        

        // You might need to keep the empty constructor for XAML designer
        public Chatwindow()
        {
            InitializeComponent();
        }

        private async void LoadChatHistory()
        {
            // Fetch from your API
            var messages = await ApiService.GetMessagesByMatchIdAsync(_currentMatch.Id);

            // Bind to your UI ListBox (make sure the ListBox name in XAML is ChatListBox)
            ChatListBox.ItemsSource = messages;
        }

        private async void OnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MessageInput.Text)) return;

            Messages newMessage = new Messages
            {
                MatchId = _currentMatch.Id,   // Use the ID
                SenderId = _currentUser.Id,   // Use the ID
                MessageText = MessageInput.Text,
                SentAt = DateTime.UtcNow
            };

            bool success = await ApiService.SendMessageAsync(newMessage);
            if (success)
            {
                MessageInput.Text = "";
                LoadChatHistory();
            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}
