using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KCK_APP.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ManageCars_Click(object sender, RoutedEventArgs e)
        {
            var manageCarsWindow = new ManageCarsWindow();
            manageCarsWindow.Show();
        }

        private void SearchCars_Click(object sender, RoutedEventArgs e)
        {
            var searchCarsWindow = new SearchCarsWindow();
            searchCarsWindow.Show();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
