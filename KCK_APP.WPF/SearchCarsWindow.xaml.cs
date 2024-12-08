using System.Windows;

using System.Collections.ObjectModel;
using System.Linq;
using KCK_APP.Controllers;
using KCK_APP.Models;
using KCK_APP.Services;

namespace KCK_APP.WPF
{
    public partial class SearchCarsWindow : Window
    {
        private readonly CarController _carController;
        private ObservableCollection<Car> _cars;

        public SearchCarsWindow()
        {
            InitializeComponent();
            _carController = new CarController(new DatabaseService());
            _cars = new ObservableCollection<Car>(_carController.GetAllCars());
            CarsListView.ItemsSource = _cars;
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            var filter = FilterTextBox.Text.ToLower();
            var filteredCars = _carController.GetAllCars()
                .Where(c => c.Make.ToLower().Contains(filter))
                .ToList();

            _cars.Clear();
            foreach (var car in filteredCars)
            {
                _cars.Add(car);
            }
        }
    }
}
