using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using KCK_APP.Controllers;
using KCK_APP.Models;
using KCK_APP.Services;

namespace KCK_APP.WPF.Views
{
    public partial class SearchCarsView : UserControl
    {
        private readonly CarController _carController;
        private ObservableCollection<Car> _cars;

        public SearchCarsView()
        {
            InitializeComponent();
            _carController = new CarController(new DatabaseService());
            LoadCars();
            LoadFilterOptions();
        }

        private void UpdateModelComboBox()
        {
            // Pobierz wybraną markę
            string selectedMake = MakeComboBox.SelectedItem as string;

            // Jeśli marka jest pusta, załaduj wszystkie modele
            var models = string.IsNullOrEmpty(selectedMake)
                ? _carController.GetUniqueModels() 
                : _carController.GetModelsByMake(selectedMake);

            // Dodaj pustą opcję na początek listy
            models.Insert(0, "");
            ModelComboBox.ItemsSource = models;
            ModelComboBox.SelectedIndex = 0; // Ustaw brak wybranego modelu
        }

        private void MakeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateModelComboBox();
        }

        private void LoadFilterOptions()
        {
            // Załaduj wszystkie dostępne marki
            var makes = _carController.GetUniqueMakes();
            makes.Insert(0, ""); // Dodaj pustą opcję na początek
            MakeComboBox.ItemsSource = makes;
            MakeComboBox.SelectedIndex = 0;

            // Załaduj wszystkie modele
            var models = _carController.GetUniqueModels();
            models.Insert(0, ""); // Dodaj pustą opcję na początek
            ModelComboBox.ItemsSource = models;
            ModelComboBox.SelectedIndex = 0;
        }

        private void LoadCars()
        {
            // Załaduj wszystkie samochody do listy
            _cars = new ObservableCollection<Car>(_carController.GetAllCars());
            CarsListView.ItemsSource = _cars;
        }

        private void ClearFilters_Click(object sender, RoutedEventArgs e)
        {
            // Resetuj filtry
            MakeComboBox.SelectedIndex = 0; // Wyzerowanie filtru marki
            ModelComboBox.SelectedIndex = 0; // Wyzerowanie filtru modelu
            MinYearTextBox.Text = string.Empty;
            MaxYearTextBox.Text = string.Empty;
            MinPriceTextBox.Text = string.Empty;
            MaxPriceTextBox.Text = string.Empty;
            MinMileageTextBox.Text = string.Empty;
            MaxMileageTextBox.Text = string.Empty;

            // Załaduj wszystkie samochody ponownie
            LoadCars();
        }

        private void FilterCars_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = true;

            // Resetowanie błędów
            MinYearError.Visibility = Visibility.Collapsed;
            MaxYearError.Visibility = Visibility.Collapsed;
            MinPriceError.Visibility = Visibility.Collapsed;
            MaxPriceError.Visibility = Visibility.Collapsed;
            MinMileageError.Visibility = Visibility.Collapsed;
            MaxMileageError.Visibility = Visibility.Collapsed;

            // Walidacja wprowadzonych danych
            if (!string.IsNullOrWhiteSpace(MinYearTextBox.Text) && !int.TryParse(MinYearTextBox.Text, out _))
            {
                MinYearError.Text = "Nieprawidłowy format roku!";
                MinYearError.Visibility = Visibility.Visible;
                isValid = false;
            }

            if (!string.IsNullOrWhiteSpace(MaxYearTextBox.Text) && !int.TryParse(MaxYearTextBox.Text, out _))
            {
                MaxYearError.Text = "Nieprawidłowy format roku!";
                MaxYearError.Visibility = Visibility.Visible;
                isValid = false;
            }

            if (!string.IsNullOrWhiteSpace(MinPriceTextBox.Text) && !decimal.TryParse(MinPriceTextBox.Text, out _))
            {
                MinPriceError.Text = "Nieprawidłowy format ceny!";
                MinPriceError.Visibility = Visibility.Visible;
                isValid = false;
            }

            if (!string.IsNullOrWhiteSpace(MaxPriceTextBox.Text) && !decimal.TryParse(MaxPriceTextBox.Text, out _))
            {
                MaxPriceError.Text = "Nieprawidłowy format ceny!";
                MaxPriceError.Visibility = Visibility.Visible;
                isValid = false;
            }

            if (!string.IsNullOrWhiteSpace(MinMileageTextBox.Text) && !decimal.TryParse(MinMileageTextBox.Text, out _))
            {
                MinMileageError.Text = "Nieprawidłowy format przebiegu!";
                MinMileageError.Visibility = Visibility.Visible;
                isValid = false;
            }

            if (!string.IsNullOrWhiteSpace(MaxMileageTextBox.Text) && !decimal.TryParse(MaxMileageTextBox.Text, out _))
            {
                MaxMileageError.Text = "Nieprawidłowy format przebiegu!";
                MaxMileageError.Visibility = Visibility.Visible;
                isValid = false;
            }

            if (!isValid)
            {
                return; // Przerwanie procesu filtrowania w przypadku błędów
            }

            // Filtrowanie samochodów
            var filteredCars = _carController.GetAllCars().AsQueryable();

            if (MakeComboBox.SelectedItem is string selectedMake && !string.IsNullOrEmpty(selectedMake))
                filteredCars = filteredCars.Where(c => c.Make == selectedMake);

            if (ModelComboBox.SelectedItem is string selectedModel && !string.IsNullOrEmpty(selectedModel))
                filteredCars = filteredCars.Where(c => c.Model == selectedModel);

            if (int.TryParse(MinYearTextBox.Text, out int minYear))
                filteredCars = filteredCars.Where(c => c.Year >= minYear);

            if (int.TryParse(MaxYearTextBox.Text, out int maxYear))
                filteredCars = filteredCars.Where(c => c.Year <= maxYear);

            if (decimal.TryParse(MinMileageTextBox.Text, out decimal minMileage))
                filteredCars = filteredCars.Where(c => c.Mileage >= minMileage);

            if (decimal.TryParse(MaxMileageTextBox.Text, out decimal maxMileage))
                filteredCars = filteredCars.Where(c => c.Mileage <= maxMileage);

            if (decimal.TryParse(MinPriceTextBox.Text, out decimal minPrice))
                filteredCars = filteredCars.Where(c => c.Price >= minPrice);

            if (decimal.TryParse(MaxPriceTextBox.Text, out decimal maxPrice))
                filteredCars = filteredCars.Where(c => c.Price <= maxPrice);

            // Aktualizacja listy wyfiltrowanych samochodów
            _cars.Clear();
            foreach (var car in filteredCars)
            {
                _cars.Add(car);
            }
        }

        private void CarsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Przejście do widoku szczegółowego samochodu
            if (CarsListView.SelectedItem is Car selectedCar)
            {
                MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
                mainWindow.MainContent.Content = new CarDetailsView(selectedCar);
            }
        }
    }
}
