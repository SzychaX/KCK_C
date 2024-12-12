using System.Windows;
using System.Windows.Controls;
using KCK_APP.Controllers;
using KCK_APP.Models;
using KCK_APP.Services;

namespace KCK_APP.WPF.Views
{
    public partial class ManageCarsView : UserControl
    {
        private readonly CarController _carController;

        public ManageCarsView()
        {
            InitializeComponent();
            _carController = new CarController(new DatabaseService());
            LoadCars();
        }

        private void LoadCars()
        {
            CarsListView.ItemsSource = _carController.GetAllCars();
        }

        private void AddCar_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
            mainWindow.MainContent.Content = new AddCarView();
        }

        private void EditCar_Click(object sender, RoutedEventArgs e)
        {
            // Pobierz wybrany samochód
            if (CarsListView.SelectedItem is Car selectedCar)
            {
                MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
                mainWindow.MainContent.Content = new EditCarView(selectedCar.Id);
            }
            else
            {
                MessageBox.Show("Wybierz samochód do edycji.");
            }
        }

        private void DeleteCar_Click(object sender, RoutedEventArgs e)
        {
            // Pobierz wybrany samochód
            if (CarsListView.SelectedItem is Car selectedCar)
            {
                var confirm = MessageBox.Show($"Czy na pewno chcesz usunąć {selectedCar.Make} {selectedCar.Model}?",
                    "Potwierdzenie", MessageBoxButton.YesNo);
                if (confirm == MessageBoxResult.Yes)
                {
                    _carController.DeleteCar(selectedCar.Id);
                    LoadCars();
                }
            }
            else
            {
                MessageBox.Show("Wybierz samochód do usunięcia.");
            }
        }

        private void CarsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CarsListView.SelectedItem is Car selectedCar)
            {
                // Debugowanie zaznaczonego auta
                System.Diagnostics.Debug.WriteLine($"Zaznaczono: {selectedCar.Make} {selectedCar.Model}");
            }
        }

        private void SearchCarById_Click(object sender, RoutedEventArgs e)
        {
            var carIdText = CarIdSearchBox.Text;

            if (string.IsNullOrEmpty(carIdText))
            {
                // Jeśli pole ID jest puste, załaduj wszystkie samochody
                LoadCars();
            }
            else
            {
                if (long.TryParse(carIdText, out long carId))
                {
                    var car = _carController.GetCarById(carId);
                    if (car != null)
                    {
                        CarsListView.ItemsSource = new[] { car };
                    }
                    else
                    {
                        MessageBox.Show("Nie znaleziono samochodu o podanym ID.");
                    }
                }
                else
                {
                    MessageBox.Show("Wprowadź poprawne ID samochodu.");
                }
            }
        }
    }
}
