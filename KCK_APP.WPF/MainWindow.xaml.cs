using System.Windows;
using KCK_APP.WPF.Views;

namespace KCK_APP.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ShowSearchCarsView(null, null); // Domyślny widok
        }

        private void ShowSearchCarsView(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new SearchCarsView();
        }

        private void ShowLoginView(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new LoginView(this);
        }

        public void ShowManageCarsView()
        {
            MainContent.Content = new ManageCarsView();
        }
    }
}