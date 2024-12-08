using System;
using System.Windows;

using KCK_APP.Models;
using KCK_APP.Controllers;
using KCK_APP.Services;

namespace KCK_APP.WPF
{
    public partial class AddCarWindow : Window
    {
        private readonly CarController _carController;

        public AddCarWindow()
        {
            InitializeComponent();
            _carController = new CarController(new DatabaseService());
        }

        private void AddCar_Click(object sender, RoutedEventArgs e)
        {
            // Walidacja wejścia użytkownika
            if (string.IsNullOrWhiteSpace(MakeTextBox.Text) ||
                string.IsNullOrWhiteSpace(ModelTextBox.Text) ||
                string.IsNullOrWhiteSpace(BodyTextBox.Text) ||
                string.IsNullOrWhiteSpace(YearTextBox.Text) ||
                string.IsNullOrWhiteSpace(MileageTextBox.Text) ||
                string.IsNullOrWhiteSpace(PowerTextBox.Text) ||
                string.IsNullOrWhiteSpace(ColorTextBox.Text) ||
                string.IsNullOrWhiteSpace(EngineTextBox.Text) ||
                string.IsNullOrWhiteSpace(PriceTextBox.Text))
            {
                MessageBox.Show("Wszystkie pola są wymagane!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Parsowanie pól z zabezpieczeniem przed błędami
            if (!int.TryParse(YearTextBox.Text, out int year))
            {
                MessageBox.Show("Rok musi być liczbą całkowitą!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(MileageTextBox.Text, out decimal mileage))
            {
                MessageBox.Show("Przebieg musi być liczbą!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price))
            {
                MessageBox.Show("Cena musi być liczbą!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Tworzenie obiektu samochodu po poprawnym parsowaniu
            var car = new Car
            {
                Make = MakeTextBox.Text.Trim(),
                Model = ModelTextBox.Text.Trim(),
                Body = BodyTextBox.Text.Trim(),
                Engine = decimal.Parse(EngineTextBox.Text),
                Color = ColorTextBox.Text.Trim(),
                HorsePower = int.Parse(PowerTextBox.Text),
                Year = year,
                Mileage = mileage,
                Price = price
            };

            // Dodanie samochodu do bazy danych
            try
            {
                _carController.AddCar(car);
                MessageBox.Show("Samochód został dodany!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas dodawania samochodu: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

