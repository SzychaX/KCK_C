using System.Windows;

using KCK_APP.Controllers;
using KCK_APP.Services;

namespace KCK_APP.WPF
{
    public partial class DeleteCarWindow : Window
    {
        private readonly CarController _carController;

        public DeleteCarWindow()
        {
            InitializeComponent();
            _carController = new CarController(new DatabaseService());
        }

        private void DeleteCar_Click(object sender, RoutedEventArgs e)
        {
            if (long.TryParse(IdTextBox.Text, out long carId))
            {
                try
                {
                    _carController.DeleteCar(carId);
                    MessageBox.Show("Samochód został usunięty!");
                    this.Close();
                }
                catch
                {
                    MessageBox.Show("Nie znaleziono samochodu o podanym ID!");
                }
            }
            else
            {
                MessageBox.Show("Nieprawidłowe ID!");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
