using System;
using System.Windows;

using KCK_APP.Controllers;
using KCK_APP.Models;
using KCK_APP.Services;

namespace KCK_APP.WPF
{
    public partial class EditCarWindow : Window
    {
        private readonly CarController _carController;
        private Car _car;

        public EditCarWindow(long carId)
        {
            InitializeComponent();
            _carController = new CarController(new DatabaseService());
            _car = _carController.GetCarById(carId) ?? throw new Exception("Nie znaleziono samochodu");
            PopulateFields();
        }

        private void PopulateFields()
        {
            IdTextBox.Text = _car.Id.ToString();
            MakeTextBox.Text = _car.Make;
            ModelTextBox.Text = _car.Model;
            PriceTextBox.Text = _car.Price.ToString();
        }

        private void SaveCar_Click(object sender, RoutedEventArgs e)
        {
            _car.Make = MakeTextBox.Text;
            _car.Model = ModelTextBox.Text;
            _car.Price = decimal.Parse(PriceTextBox.Text);

            _carController.UpdateCar(_car);
            MessageBox.Show("Zaktualizowano samochód!");
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}