using System.Windows;
using System.Windows.Controls;
using KCK_APP.Controllers;
using KCK_APP.Models;
using KCK_APP.Services;

namespace KCK_APP.WPF.Views
{
    public partial class LoginView : UserControl
    {
        private readonly UserController _userController;
        private readonly MainWindow _mainWindow;

        public LoginView(MainWindow mainWindow)
        {
            InitializeComponent();
            _userController = new UserController(new DatabaseService());
            _mainWindow = mainWindow;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            // 1) Autoryzacja
            User? user = _userController.Authenticate(
                LoginTextBox.Text,
                PasswordBox.Password);

            if (user != null)
            {
                // 2) Zapisanie zalogowanego usera w MainWindow
                _mainWindow.LoggedInUser = user;

                MessageBox.Show($"Zalogowano jako {user.Username}", 
                    "Sukces", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Information);
                if (user != null)
                {
                    _mainWindow.OnUserLoggedIn(user);
                }

                // 3) Przejście do widoku zarządzania autami i aktualizacja paska
                _mainWindow.ShowSearchCarsView(null, null);
            }
            else
            {
                MessageBox.Show("Nieprawidłowy login lub hasło!", 
                    "Błąd", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }
        
    }
}