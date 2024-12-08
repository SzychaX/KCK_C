﻿using System;
using System.Windows;
using System.Windows.Controls;
using KCK_APP.Controllers;
using KCK_APP.Models;
using KCK_APP.Services;

namespace KCK_APP.WPF.Views
{
    public partial class EditCarView : UserControl
    {
        private readonly CarController _carController;
        private Car _car;

        public EditCarView(long carId)
        {
            InitializeComponent();
            _carController = new CarController(new DatabaseService());
            _car = _carController.GetCarById(carId) ?? throw new Exception("Nie znaleziono samochodu");
            PopulateFields();
        }

        private void PopulateFields()
        {
            MakeTextBox.Text = _car.Make;
            ModelTextBox.Text = _car.Model;
            YearTextBox.Text = _car.Year.ToString();
            MileageTextBox.Text = _car.Mileage.ToString();
            EngineTextBox.Text = _car.Engine.ToString();
            HorsePowerTextBox.Text = _car.HorsePower.ToString();
            BodyTextBox.Text = _car.Body;
            ColorTextBox.Text = _car.Color;
            PriceTextBox.Text = _car.Price.ToString();
        }

        private void SaveCar_Click(object sender, RoutedEventArgs e)
        {
            _car.Make = MakeTextBox.Text;
            _car.Model = ModelTextBox.Text;
            _car.Year = int.Parse(YearTextBox.Text);
            _car.Mileage = decimal.Parse(MileageTextBox.Text);
            _car.Engine = decimal.Parse(EngineTextBox.Text);
            _car.HorsePower = int.Parse(HorsePowerTextBox.Text);
            _car.Body = BodyTextBox.Text;
            _car.Color = ColorTextBox.Text;
            _car.Price = decimal.Parse(PriceTextBox.Text);

            _carController.UpdateCar(_car);

            // Wracamy do widoku zarządzania samochodami
            var mainWindow = (MainWindow)Window.GetWindow(this);
            mainWindow.MainContent.Content = new ManageCarsView();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Wracamy do widoku zarządzania samochodami
            var mainWindow = (MainWindow)Window.GetWindow(this);
            mainWindow.MainContent.Content = new ManageCarsView();
        }
    }
}
