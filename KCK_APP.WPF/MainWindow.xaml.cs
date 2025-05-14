using System.Windows;
using KCK_APP.Models;
using KCK_APP.WPF.Views;

namespace KCK_APP.WPF
{
    public partial class MainWindow : Window
    {
        // teraz z publicznym setterem
        public User LoggedInUser { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            LoggedInUser = null;
            ShowSearchCarsView(null, null); // Domyślny widok
        }

        public void ShowSearchCarsView(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new SearchCarsView();
        }

        private void ShowRegisterView(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new RegisterView();
        }

        public void ShowLoginView(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new LoginView(this);
        }

        // przyjmujemy username tylko do wyświetlenia
        public void ShowManageCarsView(string username)
        {
            MainContent.Content = new ManageCarsView();
            LoggedInUserTextBlock.Text = $"Zalogowano jako: {username}";
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new SearchCarsView(); // wracamy do widoku domyślnego
            LoggedInUser = null;
            LoggedInUserTextBlock.Text = "Nie zalogowano"; // resetujemy info o użytkowniku
            MessageBox.Show("Wylogowano pomyślnie.", "Wylogowanie", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}