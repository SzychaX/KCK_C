using System;
using System.Windows;
using System.Windows.Controls;
using KCK_APP.Controllers;
using KCK_APP.Models;
using KCK_APP.Services;

namespace KCK_APP.WPF.Views
{
    public partial class AddCarView : UserControl
    {
        private readonly CarController _carController;

        public AddCarView()
        {
            InitializeComponent();
            _carController = new CarController(new DatabaseService());
        }

        private void AddCar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Tworzenie nowego samochodu na podstawie danych wejściowych
                var car = new Car
                {
                    Make = MakeTextBox.Text,
                    Model = ModelTextBox.Text,
                    Year = int.Parse(YearTextBox.Text),
                    Mileage = decimal.Parse(MileageTextBox.Text),
                    Engine = decimal.Parse(EngineTextBox.Text),
                    HorsePower = int.Parse(HorsePowerTextBox.Text),
                    Body = BodyTextBox.Text,
                    Color = ColorTextBox.Text,
                    Price = decimal.Parse(PriceTextBox.Text),
                    ImageUrl = ImageUrlTextBox.Text // Nowe pole
                };

                // Dodanie samochodu do bazy danych
                _carController.AddCar(car);

                MessageBox.Show("Samochód został dodany!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);

                // Powrót do widoku zarządzania samochodami
                var mainWindow = (MainWindow)Window.GetWindow(this);
                mainWindow.MainContent.Content = new ManageCarsView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Powrót do widoku zarządzania samochodami
            var mainWindow = (MainWindow)Window.GetWindow(this);
            mainWindow.MainContent.Content = new ManageCarsView();
        }
    }
}
