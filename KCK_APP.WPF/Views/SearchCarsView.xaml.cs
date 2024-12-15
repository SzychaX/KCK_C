﻿using System.Collections.ObjectModel;
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
        }

        private void LoadCars()
        {
            _cars = new ObservableCollection<Car>(_carController.GetAllCars());
            CarsListView.ItemsSource = _cars;
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
                return; // Przerwanie procesu filtrowania
            }

            var filteredCars = _carController.GetAllCars().AsQueryable();

            // Zamiana tekstów na małe litery do porównania
            if (!string.IsNullOrWhiteSpace(MakeTextBox.Text))
                filteredCars = filteredCars.Where(c => c.Make.ToLower().Contains(MakeTextBox.Text.ToLower()));

            if (!string.IsNullOrWhiteSpace(ModelTextBox.Text))
                filteredCars = filteredCars.Where(c => c.Model.ToLower().Contains(ModelTextBox.Text.ToLower()));

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

            _cars.Clear();
            foreach (var car in filteredCars)
            {
                _cars.Add(car);
            }
        }

        private void CarsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CarsListView.SelectedItem is Car selectedCar)
            {
                MainWindow mainWindow = (MainWindow)Window.GetWindow(this);
                mainWindow.MainContent.Content = new CarDetailsView(selectedCar);
            }
        }
    }
}