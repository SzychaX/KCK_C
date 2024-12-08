using System;
using System.Windows;
using System.Windows.Controls;
using KCK_APP.Controllers;
using KCK_APP.Services;

namespace KCK_APP.WPF.Views
{
    public partial class DeleteCarView : UserControl
    {
        private readonly CarController _carController;

        public DeleteCarView()
        {
            InitializeComponent();
            _carController = new CarController(new DatabaseService());
        }

        private void DeleteCar_Click(object sender, RoutedEventArgs e)
        {
            if (long.TryParse(IdTextBox.Text, out long carId))
            {
                try
                {
                    // Usuń samochód z bazy danych
                    _carController.DeleteCar(carId);
                    MessageBox.Show("Samochód został usunięty!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Powrót do widoku zarządzania samochodami
                    var mainWindow = (MainWindow)Window.GetWindow(this);
                    mainWindow.MainContent.Content = new ManageCarsView();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Nie znaleziono samochodu o podanym ID: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Nieprawidłowe ID! Wprowadź poprawne ID samochodu.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
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