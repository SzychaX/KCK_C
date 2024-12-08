using System.Windows;

namespace KCK_APP.WPF
{
    public partial class ManageCarsWindow : Window
    {
        public ManageCarsWindow()
        {
            InitializeComponent();
        }

        private void AddCar_Click(object sender, RoutedEventArgs e)
        {
            var addCarWindow = new AddCarWindow();
            addCarWindow.Show();
        }

        private void EditCar_Click(object sender, RoutedEventArgs e)
        {
            // Walidacja pola CarIdTextBox
            if (!long.TryParse(CarIdTextBox.Text, out long carId))
            {
                MessageBox.Show("Wprowadź prawidłowe ID samochodu!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Otwieranie okna edycji samochodu z przekazanym ID
            var editCarWindow = new EditCarWindow(carId);
            editCarWindow.Show();
        }

        private void DeleteCar_Click(object sender, RoutedEventArgs e)
        {
            var deleteCarWindow = new DeleteCarWindow();
            deleteCarWindow.Show();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

