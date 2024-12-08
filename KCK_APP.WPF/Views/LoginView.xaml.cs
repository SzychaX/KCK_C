using System.Windows;
using System.Windows.Controls;

namespace KCK_APP.WPF.Views
{
    public partial class LoginView : UserControl
    {
        private const string ValidLogin = "admin";
        private const string ValidPassword = "admin";
        private readonly MainWindow _mainWindow;

        public LoginView(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (LoginTextBox.Text == ValidLogin && PasswordBox.Password == ValidPassword)
            {
                MessageBox.Show("Zalogowano pomyślnie!");
                _mainWindow.ShowManageCarsView();
            }
            else
            {
                MessageBox.Show("Nieprawidłowy login lub hasło!");
            }
        }
    }
}