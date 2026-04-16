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
using interfaceapi;
using ModelDates;
using ViewModel;


namespace Meetandgreet.Pages
{
    /// <summary>
    /// Interaction logic for Settingup.xaml
    /// </summary>
    public partial class SetupPage1 : Page
    {
        // Replace 'YourExistingViewModel' with the actual name of your class
        private ViewModel viewModel;

        public SetupPage1(ViewModel vm)
        {
            InitializeComponent();
            _viewModel = vm;
            // This is the magic line that connects the XAML to your code
            this.DataContext = _viewModel;
        }

        private void NextStep_Click(object sender, RoutedEventArgs e)
        {
            // When moving to Step 2, pass the same VM along
            NavigationService.Navigate(new Settingup2(_viewModel));
        }
    }
}
