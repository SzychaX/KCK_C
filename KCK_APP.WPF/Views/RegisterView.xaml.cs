using System;
using System.Windows;
using System.Windows.Controls;
using KCK_APP.Controllers;
using KCK_APP.Services;

namespace KCK_APP.WPF.Views
{
    public partial class RegisterView : UserControl
    {
        private readonly UserController _userController;

        public RegisterView()
        {
            InitializeComponent();
            _userController = new UserController(new DatabaseService());
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Password != ConfirmBox.Password)
            {
                MessageBox.Show("Hasła się nie zgadzają!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _userController.Register(UsernameBox.Text, PasswordBox.Password);
                MessageBox.Show("Rejestracja zakończona pomyślnie!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                // Przejście do widoku logowania:
                var mainWindow = (MainWindow)Window.GetWindow(this);
                mainWindow.MainContent.Content = new LoginView(mainWindow);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się zarejestrować: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}