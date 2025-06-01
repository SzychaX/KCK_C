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
    }
}
