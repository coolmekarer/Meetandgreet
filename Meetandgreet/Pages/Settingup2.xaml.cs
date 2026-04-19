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
    /// Interaction logic for Settingup2.xaml
    /// </summary>
    public partial class Settingup2 : Page
    {
        private User currentUser;
        public Settingup2(User usr)
        {
            InitializeComponent();
            currentUser = usr;
        }

        private void Finish_Click(object sender, RoutedEventArgs e)
        {
            currentUser.Bio = aboutMe.Text;
            NavigationService.Navigate(new Homepage(currentUser));
        }
    }
}
