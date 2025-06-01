using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using KCK_APP.Controllers;
using KCK_APP.Models;

namespace KCK_APP.WPF.Views
{
    public partial class ReservationsView : UserControl
    {
        private readonly MainWindow _mainWindow;
        private readonly ReservationController _reservationController;
        private readonly CarController _carController;
        private readonly UserController _userController;

        public ReservationsView(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;

            var dbService = new Services.DatabaseService();
            _reservationController = new ReservationController(dbService);
            _carController = new CarController(dbService);
            _userController = new UserController(dbService);

            LoadReservations();
        }

        private void LoadReservations()
        {
            var allReservations = _reservationController.GetAllReservations();
            var cars = _carController.GetAllCarsReservations();
            var users = _userController.GetAllUsers();

            var reservationsToShow = _mainWindow.LoggedInUser != null && _mainWindow.LoggedInUser.Username == "admin"
                ? allReservations
                : allReservations.Where(r => r.user_id == _mainWindow.LoggedInUser.Id).ToList();

            var viewData = reservationsToShow.Select(r =>
            {
                var car = cars.FirstOrDefault(c => c.Id == r.car_id);
                var user = users.FirstOrDefault(u => u.Id == r.user_id);

                return new
                {
                    Id = r._id,
                    CarBrand = car?.Make,
                    CarModel = car?.Model,
                    StartDate = r.start_date.ToShortDateString(),
                    EndDate = r.end_date.ToShortDateString(),
                    Username = user?.Username ?? "Brak"
                };
            }).ToList();

            ReservationsList.ItemsSource = viewData;
        }

        private void DeleteReservation_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is long id)
            {
                _reservationController.DeleteReservation(id);
                LoadReservations();
            }
        }
        private void PrintMyReservations_Click(object sender, RoutedEventArgs e)
        {
            if (_mainWindow.LoggedInUser == null)
            {
                MessageBox.Show("Musisz być zalogowany, aby drukować rezerwacje.", "Brak dostępu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var userReservations = _reservationController.GetAllReservations()
                .Where(r => r.user_id == _mainWindow.LoggedInUser.Id)
                .ToList();

            if (!userReservations.Any())
            {
                MessageBox.Show("Nie masz żadnych rezerwacji do wydrukowania.", "Brak rezerwacji", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Pobierz auta i użytkowników (na wypadek, gdybyś chciał wyświetlić markę, model itp.)
            var cars = _carController.GetAllCarsReservations();
            var users = _userController.GetAllUsers();

            // Przygotuj tekst do wydruku
            var printText = new System.Text.StringBuilder();
            printText.AppendLine($"Rezerwacje użytkownika: {_mainWindow.LoggedInUser.Username}");
            printText.AppendLine(new string('-', 40));

            foreach (var r in userReservations)
            {
                var car = cars.FirstOrDefault(c => c.Id == r.car_id);
                printText.AppendLine($"Samochód: {car?.Make ?? "Nieznana marka"} {car?.Model ?? "Nieznany model"}");
                printText.AppendLine($"Okres: {r.start_date:yyyy-MM-dd} - {r.end_date:yyyy-MM-dd}");
                printText.AppendLine(new string('-', 40));
            }

            // Utwórz kontrolkę tekstową do wydruku
            var textBlock = new TextBlock
            {
                Text = printText.ToString(),
                FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                FontSize = 14,
                Margin = new Thickness(20)
            };

            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(textBlock, "Rezerwacje użytkownika");
            }
        }

    }
}
