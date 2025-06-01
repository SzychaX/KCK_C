using System;
using System.Windows;
using System.Windows.Controls;
using KCK_APP.Models;
using KCK_APP.Services;

namespace KCK_APP.WPF.Views;

public partial class ReservationFormView : UserControl
{
    private readonly long _carId;
    private readonly long _userId;
    private readonly Action _onReservationSuccess;
    private readonly DatabaseService _databaseService = new DatabaseService();

    public ReservationFormView(long carId, long userId, Action onReservationSuccess)
    {
        InitializeComponent();
        _carId = carId;
        _userId = userId;
        _onReservationSuccess = onReservationSuccess;
    }

    private void ReserveButton_Click(object sender, RoutedEventArgs e)
    {
        var startDate = StartDatePicker.SelectedDate;
        var endDate = EndDatePicker.SelectedDate;

        if (startDate == null || endDate == null)
        {
            MessageBox.Show("Wybierz daty!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var reservation = new Reservation
        {
            car_id = _carId,
            user_id = _userId,
            start_date = startDate.Value,
            end_date = endDate.Value,
        };

        DatabaseService.AddReservation(reservation);

        MessageBox.Show("Rezerwacja została zapisana!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);

        _onReservationSuccess?.Invoke();
    }
    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        var existingReservations = _databaseService.GetReservationsByCarId(_carId);

        foreach (var reservation in existingReservations)
        {
            var range = new CalendarDateRange(reservation.start_date, reservation.end_date);
            StartDatePicker.BlackoutDates.Add(range);
            EndDatePicker.BlackoutDates.Add(range);
        }

        StartDatePicker.DisplayDateStart = DateTime.Today;
        EndDatePicker.DisplayDateStart = DateTime.Today;
    }


}