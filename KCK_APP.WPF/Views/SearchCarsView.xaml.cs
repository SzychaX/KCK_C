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
        }

        private void LoadCars()
        {
            _cars = new ObservableCollection<Car>(_carController.GetAllCars());
            CarsListView.ItemsSource = _cars;
        }

        private void FilterCars_Click(object sender, RoutedEventArgs e)
        {
            var filteredCars = _carController.GetAllCars().AsQueryable();

            if (!string.IsNullOrWhiteSpace(MakeTextBox.Text))
                filteredCars = filteredCars.Where(c => c.Make.Contains(MakeTextBox.Text));

            if (!string.IsNullOrWhiteSpace(ModelTextBox.Text))
                filteredCars = filteredCars.Where(c => c.Model.Contains(ModelTextBox.Text));

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
    }
}
